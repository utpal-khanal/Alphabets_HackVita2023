using UnityEngine;

public struct DataPoint
{
	public readonly double[] inputs;
	public readonly double[] expectedOutputs;
	public readonly int label;

	public DataPoint(double[] inputs, int label, int numLabels)
	{
		this.inputs = inputs;
        int a =  GetImageLabelOutput(label);

       // this.label = label;
        this.label = a;
		expectedOutputs = CreateOneHot(a, numLabels);
	}

	public static double[] CreateOneHot(int index, int num)
	{
		double[] oneHot = new double[num];
		oneHot[index] = 1;
		return oneHot;
	}

    public static int GetImageLabelOutput(int l)
    {
        int imgLabel = l;

       /* if (l >= 48 && l <= 57)
        {
            imgLabel = l - 48;
        }
        else */
        if (l >= 65 && l <= 90)
        {
            imgLabel = l - 65;
        }
        /*else if (l >= 97 && l <= 122)
        {
            imgLabel = l - 61;
        }*/

        return imgLabel;
    }




}