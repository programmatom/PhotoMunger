/*
 *  Copyright © 2010-2016 Thomas R. Lawrence
 * 
 *  GNU General Public License
 * 
 *  This file is part of PhotoMunger
 * 
 *  PhotoMunger is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program. If not, see <http://www.gnu.org/licenses/>.
 * 
*/
using System;
using System.Diagnostics;
using System.IO;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;

namespace AdaptiveImageSizeReducer
{
    public static class PolyFit
    {
        // The following is adapted from:
        // http://numerical.recipes/forum/showthread.php?t=1481
        // An example of 2-d polynomial fitting


        // base class for a function's domain (input) parameter (e.g. scalar or tuple)
        public abstract class X
        {
        }

        // method to evaluate a np basis functions at position x
        private delegate void BasisFuncs(X x, double[] p, int np);

        // 2-dimensional domain (input) parameter, i.e. ordered pair
        public class XX : X
        {
            public readonly double x;
            public readonly double y;

            public XX(double x, double y)
            {
                this.x = x;
                this.y = y;
            }

            public override string ToString()
            {
                return String.Format("{2}{0:g5}, {1:g5}{3}", x, y, "{", "}");
            }
        }


        public class Poly2DFactory
        {
            public readonly int degree;
            public readonly int NTERMS;
            public readonly string[] labels;
            public readonly int[] xpow;
            public readonly int[] ypow;

            public Poly2DFactory(int degree)
            {
                this.degree = degree;
                for (int k = 1; k <= degree + 1; k++)
                {
                    NTERMS += k;
                }

                labels = new string[NTERMS];
                xpow = new int[NTERMS];
                ypow = new int[NTERMS];
                int i = 0;
                for (int xp = 0; xp <= degree; xp++)
                {
                    for (int yp = 0; yp <= degree; yp++)
                    {
                        if (xp + yp > degree)
                        {
                            continue;
                        }

                        xpow[i] = xp;
                        ypow[i] = yp;
                        string xx = String.Empty;
                        if (xp > 0)
                        {
                            xx = "x";
                            for (int j = 1; j < xp; j++)
                            {
                                xx = xx + "*x";
                            }
                        }
                        string yy = String.Empty;
                        if (yp > 0)
                        {
                            yy = "y";
                            for (int j = 1; j < yp; j++)
                            {
                                yy = yy + "*y";
                            }
                        }
                        labels[i] = String.Concat(xx, (xp > 0) && (yp > 0) ? "*" : String.Empty, yy);

                        i++;
                    }
                }
                Debug.Assert(i == NTERMS);
            }

            // since we have small integer exponents we can be faster than Math.Pow()
            private static double QuickPow(double x, int y)
            {
                double x2, x3, x4;
                switch (y)
                {
                    default:
                        return Math.Pow(x, y);
                    case 0:
                        return 1;
                    case 1:
                        return x;
                    case 2:
                        return x * x;
                    case 3:
                        return x * x * x;
                    case 4:
                        x2 = x * x;
                        return x2 * x2;
                    case 5:
                        x2 = x * x;
                        return x2 * x2 * x;
                    case 6:
                        x3 = x * x * x;
                        return x3 * x3;
                    case 7:
                        x3 = x * x * x;
                        return x3 * x3 * x;
                    case 8:
                        x2 = x * x;
                        x4 = x2 * x2;
                        return x4 * x4;
                }
            }

            // Threadsafe
            public void Basis(XX xx, double[] ans, int np)
            {
                Debug.Assert(np == NTERMS);
                double x = xx.x;
                double y = xx.y;
                for (int i = 0; i < NTERMS; i++)
                {
                    ans[i] = QuickPow(x, xpow[i]) * QuickPow(y, ypow[i]);
                }
            }
        }


        public class Poly2D
        {
            public readonly Poly2DFactory factory;
            private readonly double[] coeff;
            private readonly double[,] covar;
            public readonly double chiSq;

            public Poly2D(Poly2DFactory factory, double[] coeff, double[,] covar, double chiSq)
            {
                this.factory = factory;
                this.coeff = coeff;
                this.covar = covar;
                this.chiSq = chiSq;
            }

            // Threadsafe
            public double Eval(double x, double y)
            {
                double[] v = new double[factory.NTERMS];
                factory.Basis(new XX(x, y), v, factory.NTERMS);
                double r = 0;
                for (int i = 0; i < factory.NTERMS; i++)
                {
                    r += v[i] * coeff[i];
                }
                return r;
            }

            public override string ToString()
            {
                StringWriter writer = new StringWriter();
                writer.WriteLine(" {0,-9}    {1,-9} {2}", "Coeff", "Err est", "Term");
                for (int i = 1; i <= factory.NTERMS; i++)
                {
                    writer.WriteLine("({0,9:g6} +/-{1,9:g6}){2}", coeff[i], covar != null ? Math.Sqrt(covar[i, i]) : 0, factory.labels[i]);
                }
                writer.WriteLine("Chi-squared {0,-12:g6}", chiSq);
                return writer.ToString();
            }
        }


        public static Poly2D PolyFit2D(Poly2DFactory poly2dfactory, int NX, int NY, double[,] data, XX[,] pos)
        {
            int NPT = 0;
            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    if (!Double.IsNaN(data[i, j]))
                    {
                        NPT++;
                    }
                }
            }

            int NTERMS = poly2dfactory.NTERMS;

            XX[] xx = new XX[NPT];
            double[] z = new double[NPT]; // The function value at each point
            double[] sig = new double[NPT]; // Measurement errors

            // flatten: data -> z, pos -> xx
            const float SIGMIN = 0.02f;
            int rowno = 0;
            for (int i = 0; i < NX; i++)
            {
                for (int j = 0; j < NY; j++)
                {
                    if (Double.IsNaN(data[i, j]))
                    {
                        continue;
                    }

                    xx[rowno] = pos[i, j];
                    z[rowno] = data[i, j];
                    sig[rowno] = Math.Max(z[rowno] * SIGMIN, SIGMIN); // note that 1/sig[i] is used in FindFit

                    rowno++;
                }
            }
            Debug.Assert(rowno == NPT);

            // do the fit
            double[] coeff, w;
            double[,] u, v;
            double chiSq;
            FindFit(xx, z, sig, NTERMS, delegate (X _x, double[] _p, int _np) { poly2dfactory.Basis((XX)_x, _p, _np); }, out coeff, out u, out v, out w, out chiSq);
            Debug.Assert(coeff.Length == NTERMS);
            Debug.Assert(w.Length == NTERMS);
            Debug.Assert((u.GetLength(0) == NPT) && (u.GetLength(1) == NTERMS));
            Debug.Assert((v.GetLength(0) == NTERMS) && (v.GetLength(1) == NTERMS));

            // calculate covariances
            double[,] covar = null;
#if false // not used
            covar = new double[NTERMS, NTERMS];
            ComputeFitCovariance(v, NTERMS, w, covar);
#endif

            return new Poly2D(poly2dfactory, coeff, covar, chiSq);
        }

        private static void FindFit(X[] x, double[] y, double[] sig, int cFuncs, BasisFuncs funcs, out double[] coeff, out double[,] u, out double[,] v, out double[] w, out double chiSq)
        {
            int cData = x.Length;


            // - evaluate basis functions over domain set (x)
            // - scale the basis by the significance

            u = new double[cData, cFuncs];
            double[] b = new double[cData];
            double[] funcY = new double[cFuncs];
            for (int i = 0; i < cData; i++)
            {
                funcs(x[i], funcY, cFuncs);
                double sigInv = 1 / sig[i];
                for (int j = 0; j < cFuncs; j++)
                {
                    u[i, j] = funcY[j] * sigInv;
                }
                b[i] = y[i] * sigInv;
            }


            // singular value decomposition

            Matrix<double> U = Matrix<double>.Build.DenseOfArray(u);
            Svd<double> SVD = U.Svd();
            w = SVD.S.ToArray();
            Debug.Assert((SVD.VT.RowCount == cFuncs) && (SVD.VT.ColumnCount == cFuncs));
            v = SVD.VT.Transpose().ToArray();
            // Math.Net computes full mxm U matrix but we're using only the first n columns. (like LAPACK DGESVD() with JOBU='O')
            for (int i = 0; i < cData; i++)
            {
                for (int j = 0; j < cFuncs; j++)
                {
                    u[i, j] = SVD.U.At(i, j);
                }
            }


            // zero out insignificant values

            double wMax = 0;
            for (int j = 0; j < cFuncs; j++)
            {
                wMax = Math.Max(wMax, w[j]);
            }
            const double Tolerance = 1e-5;
            double cutoff = Tolerance * wMax;
            for (int j = 0; j < cFuncs; j++)
            {
                if (w[j] < cutoff)
                {
                    w[j] = 0;
                }
            }


            // solve for coefficients (uses backsubstitution)

            Vector<double> X = SVD.Solve(Vector<double>.Build.DenseOfArray(b));
            coeff = X.ToArray();


            // compute quality of fit

            chiSq = 0;
            for (int i = 0; i < cData; i++)
            {
                funcs(x[i], funcY, cFuncs);
                double sum = 0;
                for (int j = 0; j < cFuncs; j++)
                {
                    sum += coeff[j] * funcY[j];
                }
                double diff = (y[i] - sum) / sig[i];
                chiSq += diff * diff;
            }
        }
    }
}
