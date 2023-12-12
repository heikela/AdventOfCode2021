namespace Common
{
    public abstract class DynamicProgrammingProblem<TResult, TSubproblemState>
    {
        private Dictionary<TSubproblemState, TResult> MemoizedResult;

        protected DynamicProgrammingProblem()
        {
            MemoizedResult = new Dictionary<TSubproblemState, TResult>();
        }
        private TResult Memoize(TSubproblemState x, TResult result)
        {
            MemoizedResult[x] = result;
            return result;
        }

        private bool IsMemoized(TSubproblemState x)
        {
            return MemoizedResult.ContainsKey(x);
        }

        private TResult GetMemoized(TSubproblemState x)
        {
            return MemoizedResult[x];
        }

        protected abstract TResult SolveSubproblem(TSubproblemState subProblemState);

        protected TResult SolveAndMemoizeSubproblem(TSubproblemState subProblemState)
        {
            if (IsMemoized(subProblemState))
            {
                return GetMemoized(subProblemState);
            }
            else
            {
                TResult result = SolveSubproblem(subProblemState);
                Memoize(subProblemState, result);
                return result;
            }
        }

    }
}
