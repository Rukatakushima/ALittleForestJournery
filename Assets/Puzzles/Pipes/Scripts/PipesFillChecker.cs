using System.Collections.Generic;

namespace Pipes
{
    public class FillChecker
    {
        private Pipe[,] pipes;
        private List<Pipe> startPipes;

        public FillChecker(Pipe[,] pipes, List<Pipe> startPipes)
        {
            this.pipes = pipes;
            this.startPipes = startPipes;
        }

        public void CheckFill()
        {
            for (int i = 0; i < pipes.GetLength(0); i++)
            {
                for (int j = 0; j < pipes.GetLength(1); j++)
                {
                    Pipe tempPipe = pipes[i, j];
                    if (tempPipe.PipeType != 0)
                        tempPipe.IsFilled = false;
                }
            }

            Queue<Pipe> check = new Queue<Pipe>();
            HashSet<Pipe> finished = new HashSet<Pipe>();
            foreach (var pipe in startPipes)
            {
                check.Enqueue(pipe);
            }

            while (check.Count > 0)
            {
                Pipe pipe = check.Dequeue();
                finished.Add(pipe);
                List<Pipe> connected = pipe.ConnectedPipes();
                foreach (var connectedPipe in connected)
                {
                    if (!finished.Contains(connectedPipe))
                        check.Enqueue(connectedPipe);
                }
            }

            foreach (var filled in finished)
            {
                filled.IsFilled = true;
            }

            for (int i = 0; i < pipes.GetLength(0); i++)
            {
                for (int j = 0; j < pipes.GetLength(1); j++)
                {
                    Pipe tempPipe = pipes[i, j];
                    tempPipe.UpdateFillState();
                }
            }
        }
    }
}