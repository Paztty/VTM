using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HVT.VTM.Base;
using System.Threading;

namespace HVT.VTM.Program
{
    public partial class Program
    {
        #region Varialble
        // Load new model and clone to application model edit.
        private Model rootModel = new Model();
        public Model RootModel
        {
            get { return rootModel; }
            private set
            {
                rootModel = value;
                if (EditAbleModel != value)
                {
                    EditAbleModel = value;
                }
            }
        }

        // EditAbleModel change notify

        private Model EditAbleModel
        {
            get { return EditAbleModel; }
            set
            {
                EditAbleModel = value;
                NoticeOfChange();
            }
        }

        public Command Command = new Command();
        #endregion


        #region Event
        // Event model actions
        public event EventHandler EditAbleModel_Change;
        private void NoticeOfChange()
        {
            EditAbleModel_Change?.Invoke(null, null);
        }
        #endregion

        // Model init 
        public void ModelInit()
        {
            RootModel.TestState = Model.RunTestState.READY;
            Task.Run(new Action(delegate { RootModel.RunTest(); }));
        }
    }
}
