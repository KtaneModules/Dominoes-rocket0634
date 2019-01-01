using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KModkit;

public class dominoes : MonoBehaviour
{

    public KMAudio Audio;
    public KMBombInfo Bomb;
    public KMSelectable[] maindoms;
    public TextMesh[] maindomtxt;
    public Transform[] fdr1trans;
    public Transform[] fdr2trans;
    public Transform[] fdr3trans;
    public Transform[] fdr4trans;
    public GameObject[] buttons;
    public Material[] Mats;
    public GameObject[] highlights;



    private int calctype;    // 1 = addition, 2 = subtraction, 3 = multiplication, 4 = division

    private string correctpressorder;


    private static int _moduleIdCounter = 1;
    private int _moduleId = 0;



    private int[] domnums = new int[8];
    private int[] domanswers = new int[4];

    private string userinput;

    private bool filterend = false;



    void Awake()
    {

        // Referencing domino press events to event handlers :

        maindoms[0].OnInteract = delegate ()
        {
            DomOne();
            return false;
        };

        maindoms[1].OnInteract = delegate ()
        {
            DomTwo();
            return false;
        };

        maindoms[2].OnInteract = delegate ()
        {
            DomThree();
            return false;
        };

        maindoms[3].OnInteract = delegate ()
        {
            DomFour();
            return false;
        };
    }

    // Use this for initialization
    void Start ()
    {
        _moduleId = _moduleIdCounter++;
        StartModule();
	}



    void StartModule()
    {
        SetRandomNumbers();
        GetAnswerCalc();
        GetCorrectOrder();
    }





                             // Domino press event handlers :

    bool[] DomPressed = new bool[4];

    void DomOne()
    {
        if (DomPressed[0] == false && dompressed == false)
        {
            StartCoroutine("ToppleDominoes", 1);
            userinput += 1;
            Debug.LogFormat("[Dominoes #{0}] You pressed domino 1. ", _moduleId);
            DomPressed[0] = true;
        }
        
    }

    void DomTwo()
    {
        if (DomPressed[1] == false && dompressed == false)
        {
            StartCoroutine("ToppleDominoes", 2);
            userinput += 2;
            Debug.LogFormat("[Dominoes #{0}] You pressed domino 2. ", _moduleId);
            DomPressed[1] = true;
        }
    }

    void DomThree()
    {
        if (DomPressed[2] == false && dompressed == false)
        {
            StartCoroutine("ToppleDominoes", 3);
            userinput += 3;
            Debug.LogFormat("[Dominoes #{0}] You pressed domino 3. ", _moduleId);
            DomPressed[2] = true;
        }
    }

    void DomFour()
    {
        if (DomPressed[3] == false && dompressed == false)
        {
            StartCoroutine("ToppleDominoes", 4);
            userinput += 4;
            Debug.LogFormat("[Dominoes #{0}] You pressed domino 4. ", _moduleId);
            DomPressed[3] = true;
        }
    }



    // Adds random numbers from 1 to 6 on each domino's top num and bottom num :
    void SetRandomNumbers()
    {
        int rannum;
        for (int i = 0; i < 8; i++)
        {
            rannum = UnityEngine.Random.Range(1, 7);
            Debug.Log("Random number is: " + rannum);
            domnums[i] = rannum;
            maindomtxt[i].text = "";
            for (int j = 0; j < rannum; j++)
            {
                if (maindomtxt[i].text.Length == 3)
                {
                    maindomtxt[i].text += System.Environment.NewLine;
                }
                maindomtxt[i].text += "•";
            }
        }
    }


    // Does correct calculations and saves the answers to the domino calculations.          (in domanswers[] array)
    void GetAnswerCalc()
    {
        if (Bomb.GetPortCount() > 2)
        {
            calctype = 1;
            int j = 0;
            for (int i = 0; i<8; i += 2)
            {
                domanswers[j] = domnums[i] + domnums[i + 1];
                j++;
            }
        }
        else if (Bomb.GetBatteryCount() > 2)
        {
            calctype = 2;
            int j = 0;
            for (int i = 0; i<8; i+= 2)
            {
                domanswers[j] = domnums[i] - domnums[i + 1];
                j++;
            }
        }
        else if (Bomb.GetOnIndicators().Count() != 0)
        {
            calctype = 3;
            int j = 0;
            for (int i = 0; i < 8; i += 2)
            {
                domanswers[j] = domnums[i] * domnums[i + 1];
                j++;
            }
        }
        else
        {
            calctype = 4;
            int j = 0;
            for (int i = 0; i < 8; i += 2)
            {
                if (domnums[i + 1] % domnums[i] != 0)       //checks if a division calculation has a remainder
                {
                    domnums[i + 1] = domnums[i];        //and if true then sets the dividers to the same number so the result is 1
                    maindomtxt[i + 1].text = "";
                    for (int x = 0; x<domnums[i+1]; x++)
                    {
                        if (maindomtxt[i+1].text.Length == 3)
                        {
                            maindomtxt[i+1].text += System.Environment.NewLine;
                        }
                        maindomtxt[i+1].text += "•";        //and updates the displayed text
                    }
                }
                 
                domanswers[j] = domnums[i + 1] / domnums[i];
                j++;
            }
        }
        if (calctype == 1)
        {
            Debug.LogFormat("[Dominoes #{0}] The calculation type is addition. ", _moduleId);
        }
        else if (calctype == 2)
        {
            Debug.LogFormat("[Dominoes #{0}] The calculation type is subtraction. ", _moduleId);
        }
        else if (calctype == 3)
        {
            Debug.LogFormat("[Dominoes #{0}] The calculation type is multiplication. ", _moduleId);
        }
        else if (calctype == 4)
        {
            Debug.LogFormat("[Dominoes #{0}] The calculation type is division. ", _moduleId);
        }
    }



    // gets the correct domino order to press :             (and saves it in "correctpressorder" string)
    void GetCorrectOrder()
    {
        List<int> sortedansw = new List<int>();
        for (int i = 0; i < 4; i++)
        {
            sortedansw.Add(domanswers[i]);
        }
        sortedansw.Sort();

        foreach (int nums in sortedansw)
        {
            Debug.Log("---Sorted Answers : " + nums);
        }

        if (Bomb.GetOnIndicators().Count() > Bomb.GetOffIndicators().Count())
        {
            Debug.LogFormat("[Dominoes #{0}] Ascending press order. More on ind than off ind. ", _moduleId);
        }

        else if (Bomb.GetOnIndicators().Count() < Bomb.GetOffIndicators().Count())
        {
            sortedansw.Reverse();
            Debug.LogFormat("[Dominoes #{0}] Descending press order. More off ind than on ind. ", _moduleId);
            Debug.Log("Reversed sorted answers, more off ind than on ind: ");
            foreach (int nums in sortedansw)
            {
                Debug.Log("---REVind---Sorted Answers : " + nums);
            }
        }

        else if (Bomb.GetSerialNumber()[2] % 2 == 0)
        {
            sortedansw.Reverse();
            Debug.LogFormat("[Dominoes #{0}] Descending press order. Serial No. in 3rd pos is even. ", _moduleId);
            Debug.Log("Reversed sorted answers, serial No. in 3rd pos is even.");
            Debug.Log("Serial No. ------ " + Bomb.GetSerialNumber()[2]);
            foreach (int nums in sortedansw)
            {
                Debug.Log("---REVsrl---Sorted Answers : " + nums);
            }
        }

        else
        {
            Debug.LogFormat("[Dominoes #{0}] Ascending press order. ", _moduleId);
        }
        Debug.Log("Cereal Numboar -------------- " + Bomb.GetSerialNumber()[2]);





        int chk = 0;

        for (int x = 0; x < 16; x++)
        {
            while (filterend == false)           // checks if sortedanswers have the same number twice, and makes the one
            {
                chk = 0;                            // in the further position greater by 1
                for (int i = 0; i < 4; i++)         // and keeps checking until no numbers are equal to one another again
                {                                   // in the sorted answers
                    for (int s = 0; s < 4; s++)
                    {
                        if (i != s)
                        {
                            if (sortedansw[i] == sortedansw[s])
                            {
                                if (i > s)
                                {
                                    sortedansw[i]++;
                                }
                                else if (i < s)
                                {
                                    sortedansw[s]++;
                                }
                                chk++;
                            }
                        }
                    }
                }
                if (chk != 0)
                {
                    filterend = false;
                }
                else if (chk == 0)
                {
                    filterend = true;
                }
            }
        }







        Debug.Log("Calctype is : " + calctype);
        int j;

        for (int k = 0; k < 4; k++)
        {
            j = 0;
            for (int i = 0; i < 8; i += 2)
            {
                j++;

                if (calctype == 1)
                {
                    if (domnums[i+1] + domnums[i] == sortedansw[k])
                    {
                        correctpressorder += j.ToString();
                        continue;
                    }
                }


                else if (calctype == 2)
                {
                    if (domnums[i] - domnums[i + 1] == sortedansw[k])
                    {
                        correctpressorder += j.ToString();
                        continue;
                    }
                }


                else if (calctype == 3)
                {
                    if (domnums[i] * domnums[i + 1] == sortedansw[k])
                    {
                        correctpressorder += j.ToString();
                        continue;
                    }
                }


                else if (calctype == 4)
                {
                    if (domnums[i + 1] / domnums[i] == sortedansw[k])
                    {
                        correctpressorder += j.ToString();
                        continue;
                    }
                }
                Debug.Log("Correct order so far (inner loop) is: " + correctpressorder + "  j is  " + j);
            }
            Debug.Log("Correct order so far (outer loop) is: " + correctpressorder + "  j is  " + j);
        }
        Debug.LogFormat("[Dominoes #{0}] The correct press order is: {1}", _moduleId, correctpressorder);

        for (int i = 0; i < 8; i++)
        {
            Debug.Log("DomNum's are " + domnums[i] + "  for position: " + (i+1));
        }
        for (int i = 0; i < 4; i++)
        {
            Debug.Log("DomAnswers are " + domanswers[i] + "  for number  :" + (i+1));
        }
    }







    private bool[] toppledrow = new bool[4];
    private bool toppling = true;
    private bool dompressed = false;

    private const float xrot = 75f, y = 0.001f, z = 0.01f, hxrot = xrot / 2f, hy = y / 2f, hz = z / 2f;
    private const int range = 1;

    IEnumerator ToppleDominoes(int row)                //A coroutine which "topples" the dominoes.
    {
            for (int i = 7; i > -1; i--)
            {
                if (row == 1 && toppledrow[0] == false)
                {
                if (i==7)
                {
                    highlights[0].SetActive(false);
                }
                dompressed = true;
                for (int k = 0; k < range; k++)
                {
                    if (i==7)
                    {
                        fdr1trans[i].Rotate(new Vector3(xrot, 0, 0));
                        fdr1trans[i].Translate(new Vector3(0f, y, z));
                        fdr1trans[i-1].Rotate(new Vector3(hxrot , 0, 0));
                        fdr1trans[i-1].Translate(new Vector3(0f, hy, hz));
                    }
                    else if (i==0)
                    {
                        fdr1trans[i].Rotate(new Vector3(hxrot, 0, 0));
                        fdr1trans[i].Translate(new Vector3(0f, hy, hz));
                    }
                    else
                    {
                        fdr1trans[i].Rotate(new Vector3(hxrot, 0, 0));
                        fdr1trans[i].Translate(new Vector3(0f, hy, hz));
                        fdr1trans[i-1].Rotate(new Vector3(hxrot, 0, 0));
                        fdr1trans[i-1].Translate(new Vector3(0f, hy, hz));
                    }
                    yield return null;
                }
                if (i == 0)
                {
                    buttons[0].GetComponentInChildren<MeshRenderer>().material = Mats[0];
                    toppledrow[0] = true;
                }
                dompressed = false;
                }


                else if (row == 2 && toppledrow[1] == false)
                {
                if (i == 7)
                {
                    highlights[1].SetActive(false);
                }
                dompressed = true;
                for (int k = 0; k < range; k++)
                {
                    if (i == 7)
                    {
                        fdr2trans[i].Rotate(new Vector3(xrot, 0, 0));
                        fdr2trans[i].Translate(new Vector3(0f, y, z));
                        fdr2trans[i - 1].Rotate(new Vector3(hxrot, 0, 0));
                        fdr2trans[i - 1].Translate(new Vector3(0f, hy, hz));
                    }
                    else if (i == 0)
                    {
                        fdr2trans[i].Rotate(new Vector3(hxrot, 0, 0));
                        fdr2trans[i].Translate(new Vector3(0f, hy, hz));
                    }
                    else
                    {
                        fdr2trans[i].Rotate(new Vector3(hxrot, 0, 0));
                        fdr2trans[i].Translate(new Vector3(0f, hy, hz));
                        fdr2trans[i - 1].Rotate(new Vector3(hxrot, 0, 0));
                        fdr2trans[i - 1].Translate(new Vector3(0f, hy, hz));
                    }
                    yield return null;
                }
                if (i == 0)
                {
                    buttons[1].GetComponentInChildren<MeshRenderer>().material = Mats[0];
                    toppledrow[1] = true;
                }
                dompressed = false;
            }


                else if (row == 3 && toppledrow[2] == false)
                {
                if (i == 7)
                {
                    highlights[2].SetActive(false);
                }
                dompressed = true;
                for (int k = 0; k < range; k++)
                {
                    if (i == 7)
                    {
                        fdr3trans[i].Rotate(new Vector3(xrot, 0, 0));
                        fdr3trans[i].Translate(new Vector3(0f, y, z));
                        fdr3trans[i - 1].Rotate(new Vector3(hxrot, 0, 0));
                        fdr3trans[i - 1].Translate(new Vector3(0f, hy, hz));
                    }
                    else if (i == 0)
                    {
                        fdr3trans[i].Rotate(new Vector3(hxrot, 0, 0));
                        fdr3trans[i].Translate(new Vector3(0f, hy, hz));
                    }
                    else
                    {
                        fdr3trans[i].Rotate(new Vector3(hxrot, 0, 0));
                        fdr3trans[i].Translate(new Vector3(0f, hy, hz));
                        fdr3trans[i - 1].Rotate(new Vector3(hxrot, 0, 0));
                        fdr3trans[i - 1].Translate(new Vector3(0f, hy, hz));
                    }
                    yield return null;
                }
                if (i == 0)
                {
                    buttons[2].GetComponentInChildren<MeshRenderer>().material = Mats[0];
                    toppledrow[2] = true;
                }
                dompressed = false;
            }


                else if (row == 4 && toppledrow[3] == false)
                {
                if (i == 7)
                {
                    highlights[3].SetActive(false);
                }
                dompressed = true;
                for (int k = 0; k < range; k++)
                {
                    if (i == 7)
                    {
                        fdr4trans[i].Rotate(new Vector3(xrot, 0, 0));
                        fdr4trans[i].Translate(new Vector3(0f, y, z));
                        fdr4trans[i - 1].Rotate(new Vector3(hxrot, 0, 0));
                        fdr4trans[i - 1].Translate(new Vector3(0f, hy, hz));
                    }
                    else if (i == 0)
                    {
                        fdr4trans[i].Rotate(new Vector3(hxrot, 0, 0));
                        fdr4trans[i].Translate(new Vector3(0f, hy, hz));
                    }
                    else
                    {
                        fdr4trans[i].Rotate(new Vector3(hxrot, 0, 0));
                        fdr4trans[i].Translate(new Vector3(0f, hy, hz));
                        fdr4trans[i - 1].Rotate(new Vector3(hxrot, 0, 0));
                        fdr4trans[i - 1].Translate(new Vector3(0f, hy, hz));
                    }
                    yield return null;
                }
                if (i == 0)
                {
                    buttons[3].GetComponentInChildren<MeshRenderer>().material = Mats[0];
                    toppledrow[3] = true;
                }
                dompressed = false;
            }
            }
        if (toppledrow[0] == true && toppledrow[1] == true && toppledrow[2] == true && toppledrow[3] == true)
        {
            yield return new WaitForSeconds(0.01f);
            toppling = false;
        }
        if (userinput.Length == 4)
        {
            CheckUserInput();
        }
    }





    IEnumerator ResetDoms()
    {
        while (toppling == true)
        {
            yield return new WaitForSeconds(0.1f);
            continue;
        }
        for (int i = 7; i > -1; i--)
        {
            if (i==7)
            {
                fdr1trans[i].Translate(new Vector3(0f, -y, -z));
                fdr2trans[i].Translate(new Vector3(0f, -y, -z));
                fdr3trans[i].Translate(new Vector3(0f, -y, -z));
                fdr4trans[i].Translate(new Vector3(0f, -y, -z));
            }
            else
            {
                fdr1trans[i].Translate(new Vector3(0f, -y * 3.5f, -z/1.2f));
                fdr2trans[i].Translate(new Vector3(0f, -y * 3.5f, -z/1.2f));
                fdr3trans[i].Translate(new Vector3(0f, -y * 3.5f, -z/1.2f));
                fdr4trans[i].Translate(new Vector3(0f, -y * 3.5f, -z/1.2f));
            }
            fdr1trans[i].Rotate(new Vector3(-xrot, 0f, 0f));
            fdr2trans[i].Rotate(new Vector3(-xrot, 0f, 0f));
            fdr3trans[i].Rotate(new Vector3(-xrot, 0f, 0f));
            fdr4trans[i].Rotate(new Vector3(-xrot, 0f, 0f));
        }
        buttons[0].GetComponentInChildren<MeshRenderer>().material = Mats[2];
        buttons[1].GetComponentInChildren<MeshRenderer>().material = Mats[2];
        buttons[2].GetComponentInChildren<MeshRenderer>().material = Mats[2];
        buttons[3].GetComponentInChildren<MeshRenderer>().material = Mats[2];

        correctpressorder = "";

        for (int i=0; i<4; i++)
        {
            highlights[i].SetActive(true);
        }


        for (int i=0; i<4; i++)
        {
            toppledrow[i] = false;
        }

        for (int i=0; i<4; i++)
        {
            DomPressed[i] = false;
        }
        StartCoroutine("RedFlashButtons");
        GetComponent<KMBombModule>().HandleStrike();
        StartModule();
    }




    IEnumerator RedFlashButtons()
    {
        for (int i = 0; i<3; i++)
        {
            for (int k = 0; k<4; k++)
            {
                buttons[k].GetComponentInChildren<MeshRenderer>().material = Mats[1];
                yield return new WaitForSeconds(0.01f);
                buttons[k].GetComponentInChildren<MeshRenderer>().material = Mats[2];
                yield return new WaitForSeconds(0.01f);
            }
        }
    }





    void CheckUserInput()
    {
        if (int.Parse(correctpressorder) == int.Parse(userinput))
        {
            GetComponent<KMBombModule>().HandlePass();
        }
        else
        {
            userinput = "";
            correctpressorder = "";
            filterend = false;
            StartCoroutine("ResetDoms");
        }
    }



    /*KMSelectable[] ProcessTwitchCommand(string command)
    {
        command = command.ToLowerInvariant();
        char[] button = command.Replace("press", "").Replace(",", "").Replace(" ", "").ToCharArray();
        //".Any" returns any value that is true based on the lambda function. In this case, we want to make sure the char is between 1 and 4.
        //Since chars represent int values, subtracting '0' from a number represented by a char will return the proper number
        //InRange is an extension provided by the community - it can be found in GeneralExtensions.cs
        if (!command.Contains("press") || button.Count() != 4 || button.Any(x => !(x - '0').InRange(1, 4))) return null;
        List<KMSelectable> presses = new List<KMSelectable>();
        foreach (int num in button.Select(x => x - '0'))
        {
            presses.Add(maindoms[num - 1]);
        }
        return presses.ToArray();
    }*/
}
