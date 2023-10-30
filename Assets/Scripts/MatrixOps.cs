using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatrixOps : MonoBehaviour
{
    public static int[,] Multiply(int[,] A, int[,] B)
    {
        int rA = A.GetLength(0);
        int cA = A.GetLength(1);
        int rB = B.GetLength(0);
        int cB = B.GetLength(1);

        if (cA != rB)
        {
            return null;
        }
        else
        {
            int temp = 0;
            int[,] result = new int[rA, cB];

            for (int i = 0; i < rA; i++)
            {
                for (int j = 0; j < cB; j++)
                {
                    temp = 0;
                    for (int k = 0; k < cA; k++)
                    {
                        temp += A[i, k] * B[k, j];
                    }
                    result[i, j] = temp;
                }
            }

            return result;
        }
    }

    public static float[,] MultiplyDouble(float[,] A, int[,] B)
    {
        int rA = A.GetLength(0);
        int cA = A.GetLength(1);
        int rB = B.GetLength(0);
        int cB = B.GetLength(1);

        if (cA != rB)
        {
            return null;
        }
        else
        {
            float temp = 0;
            float[,] result = new float[rA, cB];

            for (int i = 0; i < rA; i++)
            {
                for (int j = 0; j < cB; j++)
                {
                    temp = 0;
                    for (int k = 0; k < cA; k++)
                    {
                        temp += A[i, k] * B[k, j];
                    }
                    result[i, j] = temp;
                }
            }

            return result;
        }
    }
}
