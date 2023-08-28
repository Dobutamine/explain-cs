using System;
namespace ExplainCoreLib.functions
{
    public static class BrentRootFindingProcedure
    {
        public static double BrentRootFinding(Func<double, double> f, double x0, double x1, int maxIter, double tolerance)
        {
            double fx0 = f(x0);
            double fx1 = f(x1);

            if (fx0 * fx1 > 0)
                return -1;

            if (Math.Abs(fx0) < Math.Abs(fx1))
            {
                (x1, x0) = (x0, x1);
                (fx0, fx1) = (fx1, fx0);
            }

            double x2 = x0;
            double fx2 = fx0;
            bool mflag = true;
            int stepsTaken = 0;

            while (stepsTaken < maxIter)
            {
                if (Math.Abs(fx0) < Math.Abs(fx1))
                {
                    (x0, x1) = (x1, x0);
                    (fx0, fx1) = (fx1, fx0);
                }

                fx0 = f(x0);
                fx1 = f(x1);
                fx2 = f(x2);

                double L0, L1, L2, newPoint;
                if (fx0 != fx2 && fx1 != fx2)
                {
                    L0 = x0 * fx1 * fx2 / ((fx0 - fx1) * (fx0 - fx2));
                    L1 = x1 * fx0 * fx2 / ((fx1 - fx0) * (fx1 - fx2));
                    L2 = x2 * fx1 * fx0 / ((fx2 - fx0) * (fx2 - fx1));
                    newPoint = L0 + L1 + L2;
                }
                else
                {
                    newPoint = x1 - (fx1 * (x1 - x0) / (fx1 - fx0));
                }



                double fnew = f(newPoint);
                double d = x2;
                x2 = x1;

                if ((fx0 * fnew) < 0)
                {
                    x1 = newPoint;
                }
                else
                {
                    x0 = newPoint;
                }

                stepsTaken++;

                if (Math.Abs(fnew) < tolerance)
                {
                    return newPoint;
                }

                if (newPoint < (3 * x0 + x1) / 4 || newPoint > x1 ||
                    (mflag && Math.Abs(newPoint - x1) >= Math.Abs(x1 - x2) / 2) ||
                    (!mflag && Math.Abs(newPoint - x1) >= Math.Abs(x2 - d) / 2) ||
                    (mflag && Math.Abs(x1 - x2) < tolerance) ||
                    (!mflag && Math.Abs(x2 - d) < tolerance))
                {
                    newPoint = (x0 + x1) / 2;
                    mflag = true;
                }
                else
                {
                    mflag = false;
                }
            }

            return -1; // Failed to find a root within the maximum number of iterations
        }
    }

}

