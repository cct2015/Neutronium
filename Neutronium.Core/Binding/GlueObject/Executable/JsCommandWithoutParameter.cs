﻿using System;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.MVVMComponents;

namespace Neutronium.Core.Binding.GlueObject.Executable
{
    public class JsCommandWithoutParameter : JsCommandBase, IJsCsCachableGlue, IExecutableGlue
    {
        public object CValue => _Command;
        private readonly ICommandWithoutParameter _Command;

        void IJsCsCachableGlue.SetJsId(uint jsId) => base.SetJsId(jsId);

        public JsCommandWithoutParameter(HtmlViewContext context, IJavascriptToCSharpConverter converter, ICommandWithoutParameter command) :
            base(context, converter)
        {
            _Command = command;
            _CanExecute = _Command.CanExecute;
        }

        public virtual void SetJsValue(IJavascriptObject value, IJavascriptSessionCache sessionCache)
        {
            SetJsValue(value);
            sessionCache.Cache(this);
        }

        public void VisitDescendants(Func<IJsCsGlue, bool> visit)
        {
            visit(this);
        }

        public override void ListenChanges()
        {
            _Command.CanExecuteChanged += _Command_CanExecuteChanged;
        }

        public override void UnListenChanges()
        {
            _Command.CanExecuteChanged -= _Command_CanExecuteChanged;
        }

        public override void Execute(IJavascriptObject[] e)
        {
            UiDispatcher.Dispatch(() => _Command.Execute());
        }

        private void _Command_CanExecuteChanged(object sender, EventArgs e)
        {
            ComputeCanExecute();
        }

        public override void CanExecuteCommand(params IJavascriptObject[] e)
        {
            ComputeCanExecute();
        }

        private async void ComputeCanExecute()
        {
            await UiDispatcher.RunAsync(() => _CanExecute = _Command.CanExecute);
            UpdateCanExecuteValue();
        }
    }
}
