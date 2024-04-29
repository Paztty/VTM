using HVT.VTM.Base;
using HVT.Utility;
using System;

namespace HVT.VTM.Program
{
    public partial class Program
    {
        public event EventHandler EditModel_OnSave;
        public void OnEditModelSave()
        {
            EditModel_OnSave?.Invoke(EditModel, null);
        }

        public event EventHandler EditModel_OnLoaded;
        public void OnEditModelLoaded()
        {
            EditModel_OnLoaded?.Invoke(EditModel, null);
        }

        private Model editModel = new Model();
        public Model EditModel
        {
            get { return editModel; }
            set
            {
                if (value != editModel)
                {
                    editModel = value;
                }
            }
        }
    }
}
