using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVT.VTM.Machine
{
    public class PBA
    {
        public List<TestPCB> PCBs { get; set; }
        private string[] PCBnaming = { "A", "B", "C", "D", "E", "F", "G", "H" };

        public PBA(Core.Model model)
        {
            PCBs.Clear();
            for (int i = 0; i < model.contruction.PCB_Count; i++)
            {
                if (i < PCBnaming.Length)
                {
                    PCBs.Add(new TestPCB()
                    {
                        Name = PCBnaming[i],
                    });
                }

            }
            
        
        }
    }
}
