using System.Collections.Generic;
using System.Linq;

namespace Pipes
{
    public class FillChecker
    {
        private readonly Pipe[,] _pipes;
        private readonly List<Pipe> _startPipes;

        public FillChecker(Pipe[,] pipes, List<Pipe> startPipes)
        {
            _pipes = pipes;
            _startPipes = startPipes;
        }

        public void CheckFill()
        {
            for (int i = 0; i < _pipes.GetLength(0); i++)
            {
                for (int j = 0; j < _pipes.GetLength(1); j++)
                {
                    Pipe tempPipe = _pipes[i, j];
                    if (tempPipe.pipeType != 0)
                        tempPipe.isFilled = false;
                }
            }

            Queue<Pipe> check = new Queue<Pipe>();
            HashSet<Pipe> finished = new HashSet<Pipe>();
            foreach (var pipe in _startPipes)
            {
                check.Enqueue(pipe);
            }

            while (check.Count > 0)
            {
                Pipe pipe = check.Dequeue();
                finished.Add(pipe);
                List<Pipe> connected = pipe.ConnectedPipes();
                foreach (var connectedPipe in connected.Where(connectedPipe => !finished.Contains(connectedPipe)))
                {
                    check.Enqueue(connectedPipe);
                }
            }

            foreach (var filled in finished)
            {
                filled.isFilled = true;
            }

            for (int i = 0; i < _pipes.GetLength(0); i++)
            {
                for (int j = 0; j < _pipes.GetLength(1); j++)
                {
                    Pipe tempPipe = _pipes[i, j];
                    tempPipe.UpdateFillState();
                }
            }
        }
    }
}