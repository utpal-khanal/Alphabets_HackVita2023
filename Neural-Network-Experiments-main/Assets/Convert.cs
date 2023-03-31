using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using static Unity.VisualScripting.Member;
using System;

public struct TextureData {
    public Texture2D texture;
    public int label;
}

public class Convert : MonoBehaviour
{
    Texture2D pngTexture;
    [SerializeField] ImageLoader imageLoader;
    [SerializeField] ImageViewer imageViewer;
    [SerializeField] ImageProcessor imageProcessor;
    [SerializeField] byte[] pngbyte;
    List<TextureData> textureDataList = new List<TextureData>();

    List<Texture2D> pngList;
    public List<Image> imageList = new List<Image>();

    // Start is called before the first frame update
    void Start()
    {


        /* char c = (char)104;
         int c = '0';
         Debug.Log(c + " my char");*/





        string filePath = Application.dataPath + "/labels.csv";

        StreamReader reader = new StreamReader(filePath);

        while (!reader.EndOfStream)
        {
            string line = reader.ReadLine();
            string[] values = line.Split(',');

            //string imgnames = values[0];
            // imgnames = imgnames.Substring(4, 10);

            // Do something with the values
            // For example, print them to the console
            string imgName = values[0];
            string str = values[1];
            int label = str[0];


            // Debug.Log("V1" + imgName + ", Value2 " + label);
            pngTexture = Resources.Load<Texture2D>($"Img/{imgName}");
            TextureData textureData = new TextureData();
            textureData.texture = pngTexture;
            textureData.label = label;
            textureDataList.Add(textureData);

            // pngTexture = Resources.Load<Texture2D>($"df/fasdfas");
            // pngList.Add(pngTexture);

        }

        reader.Close();
        Debug.Log(textureDataList.Count + " texture size");

        // pngTexture =  Resources.Load<Texture2D>("Textures/texture01");

        for (int i = 0; i < textureDataList.Count; i++)
        {
            int sourceMipLevel = 0;
            Color[] pixels = textureDataList[i].texture.GetPixels(sourceMipLevel);
            //Debug.Log(pixels.Length + " pix length");
            double[] pixelsValue = new double[pixels.Length];

            for (int k = 0; k < pixels.Length; k++)
            {
                pixelsValue[k] = pixels[k].grayscale;
            }

            //Debug.Log(pngList.Count + " list size");


            Image image = new Image(28, true, pixelsValue, textureDataList[i].label);
            imageList.Add(image);
        }


        Debug.Log(imageList.Count + " imgList size");
        //imageViewer.DisplayImage(image);

        //imageProcessor.SetMyImage(image);







    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
