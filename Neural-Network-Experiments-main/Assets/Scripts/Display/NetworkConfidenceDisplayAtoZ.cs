using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkConfidenceDisplayAtoZ : MonoBehaviour
{

	public TextAsset networkFile;
	public static string predictedLabel;

	public MeshRenderer display;
	public TMPro.TMP_Text labelsUI;
	public TMPro.TMP_Text confidenceUI;

	ImageLoader loader;
	DrawingControllerAtoZ drawingController;
	NeuralNetwork network;

	public TMPro.TMP_Text Accuracy;///////////////////////////////////////////////////////////// 



	void Start()
	{
		drawingController = FindObjectOfType<DrawingControllerAtoZ>();
		network = NetworkSaveData.LoadNetworkFromData(networkFile.text);
		loader = FindObjectOfType<ImageLoader>();
	}


	void Update()
	{
		RenderTexture digitRenderTexture = drawingController.RenderOutputTexture();
		Image image = ImageHelper.TextureToImage(digitRenderTexture, 0);
		(int prediction, double[] outputs) = network.Classify(image.pixelValues);

		UpdateDisplay(image, outputs, prediction);


	}

	void UpdateDisplay(Image image, double[] outputs, int prediction)
	{
		predictedLabel = loader.LabelNames[prediction];

		var rankedLabels = new List<RankedLabel>();
		double s = 0;

		for (int i = 0; i < outputs.Length; i++)
		{
			var r = new RankedLabel() { name = loader.LabelNames[i], score = (float)outputs[i] };
			rankedLabels.Add(r);
			s += outputs[i];
		}

		rankedLabels.Sort((a, b) => b.score.CompareTo(a.score));
		labelsUI.text = "<color=#ffffff>";
		confidenceUI.text = "<color=#ffffff>";
		for (int i = 0; i < outputs.Length; i++)
		{
			labelsUI.text += rankedLabels[i].name + "\n" + ((i == 0) ? "</color>" : "");
			confidenceUI.text += rankedLabels[i].Text + "\n" + ((i == 0) ? "</color>" : ""); ;
		}

		display.material.mainTexture = image.ConvertToTexture2D();

		for(int i = 0; i < rankedLabels.Count; i++) //////////////////////////////////////////////
        {
			if(ChangeStringToInt(rankedLabels[i].name) == DrawingControllerAtoZ.currentQuestion)///////////////////////////////////////////////
            {
				Accuracy.text = ((int)(((rankedLabels[i].score)*100))).ToString() + "%";///////////////////////////////////////////
				break;/////////////////////////////////////////
            }
        }
	}

	public struct RankedLabel
	{
		public string name;
		public float score;

		public string Text
		{
			get
			{
				return $"{score * 100:0.00}%";
			}
		}
	}

	

	int ChangeStringToInt(string a)///////////////////////////////////////////////////////////////////
    {
		if (a == "A") return 0;
		else if (a == "B") return 1;
		else if (a == "C") return 2;
		else if (a == "D") return 3;
		else if (a == "E") return 4;
		else if (a == "F") return 5;
		else if (a == "G") return 6;
		else if (a == "H") return 7;
		else if (a == "I") return 8;
		else if (a == "J") return 9;
		else if (a == "K") return 10;
		else if (a == "L") return 11;
		else if (a == "M") return 12;
		else if (a == "N") return 13;
		else if (a == "O") return 14;
		else if (a == "P") return 15;
		else if (a == "Q") return 16;
		else if (a == "R") return 17;
		else if (a == "S") return 18;
		else if (a == "T") return 19;
		else if (a == "U") return 20;
		else if (a == "V") return 21;
		else if (a == "W") return 22;
		else if (a == "X") return 23;
		else if (a == "Y") return 24;
		return 25;
		
	}
}
