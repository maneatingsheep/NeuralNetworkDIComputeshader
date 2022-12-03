using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class InputDataManager : MonoBehaviour
{

    private const string TrainImages = "Model/train-images.idx3-ubyte";
    private const string TrainLabels = "Model/train-labels.idx1-ubyte";
    private const string TestImages = "Model/t10k-images.idx3-ubyte";
    private const string TestLabels = "Model/t10k-labels.idx1-ubyte";

    private Image[] TrainingData;
    private Image[] TestData;

    public void LoadData() {
        TrainingData = Read(TrainImages, TrainLabels);
        TestData = Read(TestImages, TestLabels);
    }

    private Image[] Read(string imagesPath, string labelsPath) {
        BinaryReader labels = new BinaryReader(new FileStream(labelsPath, FileMode.Open));
        BinaryReader images = new BinaryReader(new FileStream(imagesPath, FileMode.Open));

        

        int magicNumber = images.ReadBigInt32();
        int numberOfImages = images.ReadBigInt32();
        int width = images.ReadBigInt32();
        int height = images.ReadBigInt32();

        Image[] result = new Image[numberOfImages];

        int magicLabel = labels.ReadBigInt32();
        int numberOfLabels = labels.ReadBigInt32();

        for (int i = 0; i < numberOfImages; i++) {
            var bytes = images.ReadBytes(width * height);
            var arr = new byte[height, width];

            arr.ForEach((j, k) => arr[j, k] = bytes[j * height + k]);

            result[i] = new Image() {
                Data = arr,
                Label = labels.ReadByte()
            };
        }

        labels.Dispose();
        images.Dispose();

        return result;
    }

    public Image GetRandomImage(bool isTraining) {
        if (isTraining) {
            return TrainingData[UnityEngine.Random.Range(0, TrainingData.Length)];
        } else {
            return TestData[UnityEngine.Random.Range(0, TrainingData.Length)];
        }
    }
}

public class Image {
    public byte Label { get; set; }
    public byte[,] Data { get; set; }
}

public static class Extensions {
    public static int ReadBigInt32(this BinaryReader br) {
        var bytes = br.ReadBytes(sizeof(Int32));
        if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
        return BitConverter.ToInt32(bytes, 0);
    }

    public static void ForEach<T>(this T[,] source, Action<int, int> action) {
        for (int w = 0; w < source.GetLength(0); w++) {
            for (int h = 0; h < source.GetLength(1); h++) {
                action(w, h);
            }
        }
    }
}

//Usage:

    /*foreach (var image in MnistReader.ReadTrainingData()) {
        //use image here     
    }
    or

    foreach (var image in MnistReader.ReadTestData()) {
        //use image here     
    }*/
