using System.Linq;
using OzAlgo.LoopSolver;
using System.Collections.Generic;

namespace OzAlgo
{
    public class IterativeGeneralizationSolver
    {
        private string _varName;
        private int _previousN;
        private List<LoopEquation2> _previousLoops;

        public IterativeGeneralizationSolver(string varName)
        {
            _varName = varName;
        }

        public void Solve(int N, List<LoopEquation2> loops)
        {



            _previousLoops = loops.ToList();
            _previousN = N;
        }
    }
}
