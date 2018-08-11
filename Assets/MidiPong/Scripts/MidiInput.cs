﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MidiInput : MonoBehaviour
{
    static MidiInput instance;
    MidiReceiver receiver;
    Dictionary<int, float> controllers;
    Dictionary<int, float> pads;
    bool toLearn;
    int learnt;

    public static int LearntChannel {
        get { return instance.learnt; }
    }

    public static float GetKnob (int channel)
    {
        if (instance.controllers.ContainsKey (channel)) {
            return instance.controllers [channel];
        } else {
            return -1.0f;
        }
    }

    public static float GetPad (int channel)
    {
        if (instance.pads.ContainsKey (channel)) {
            return instance.pads [channel];
        } else {
            return -1.0f;
        }
    }


    public static void StartLearn ()
    {
        instance.learnt = -1;
        instance.toLearn = true;
    }

    void Awake ()
    {
        instance = this;
        learnt = -1;
    }

    void Start ()
    {
        receiver = FindObjectOfType (typeof(MidiReceiver)) as MidiReceiver;
        controllers = new Dictionary<int, float> ();
        pads = new Dictionary<int, float> ();
    }

    private static int ConvertToArturiaKnobs(int number) {
        if (number == 7) 
            return 0;
        if (number == 10) 
            return 1;
        if (number == 74)
            return 2;
        if (number == 71) 
            return 3;
        if (number == 76)
            return 4;
        if (number == 77)
            return 5;
        if (number == 93)
            return 6;
        if (number == 73)
            return 7;
        if (number == 75)
            return 8;
        if (number == 114)
            return 9;
        if (number == 18)
            return 10;
        if (number == 19)
            return 11;
        if (number == 16)
            return 12;
        if (number == 17)
            return 13;
        if (number == 91)
            return 14;
        if (number == 79)
            return 15;
        if (number == 72)
            return 16;
        else 
            return number;
    }

    private static int ConvertToArturiaPads(int number) {
        if (number == 44) 
            return 1;
        if (number == 45)
            return 2;
        if (number == 46) 
            return 3;
        if (number == 47)
            return 4;
        if (number == 48)
            return 5;
        if (number == 49)
            return 6;
        if (number == 50)
            return 7;
        if (number == 51)
            return 8;
        if (number == 36)
            return 9;
        if (number == 37)
            return 10;
        if (number == 38)
            return 11;
        if (number == 39)
            return 12;
        if (number == 40)
            return 13;
        if (number == 41)
            return 14;
        if (number == 42)
            return 15;
        if (number == 43)
            return 16;
        else 
            return number;
    }

    void Update ()
    {
        while (!receiver.IsEmpty) {
            var message = receiver.PopMessage ();
            if (message.status == 0xb0) {
                controllers [ConvertToArturiaKnobs(message.data1)] = 1.0f / 127 * message.data2;
                if (toLearn) {
                    learnt = message.data1;
                    toLearn = false;
                }
            }
            if (message.status == 0x90) {
                Debug.Log("status: " + message.status + ", data1: " + message.data1 + ", data2: " + message.data2 + ", jes!");
                pads [ConvertToArturiaPads(message.data1)] = 1;
            }
            if (message.status == 0x80) {
                Debug.Log("status: " + message.status + ", data1: " + message.data1 + ", data2: " + message.data2 + ", jes!");
                pads [ConvertToArturiaPads(message.data1)] = 0;
            }
        }
    }
}