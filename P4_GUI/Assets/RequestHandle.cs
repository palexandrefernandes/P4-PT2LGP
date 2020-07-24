using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System.IO;
using System.Diagnostics;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.Threading;

public class RequestHandle : MonoBehaviour
{
    public Dictionary<string, Motion> animations = new Dictionary<string, Motion>();
    public Motion iddle;
    public Animator animator;
    public TMP_InputField textfield;
    public Button submit;
    public GameObject ui;
    public TMP_Text representation;
    public GameObject errorHolder;
    public TMP_Text error;
    public GameObject spinner;
    public GameObject representationHolder;

    private const String SERVER_URI = "http://localhost:5000/api/";
    private bool isRequesting = false;
    private Queue<string> sequence = new Queue<string>();

    private AnimationState animationSequence;


    // Start is called before the first frame update
    void Start()
    {
        submit.onClick.AddListener(OnClickHandler);
    }

    void OnClickHandler()
    {
        if(textfield.text != "")
            StartCoroutine(GetRequest(textfield.text.ToString()));
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator GetRequest(String sentence)
    {
        ui.SetActive(false);
        spinner.SetActive(true);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(SERVER_URI+"translate?sentence="+sentence))
        {
            // Request and wait for the desired page.
            isRequesting = true;
            yield return webRequest.SendWebRequest();
            spinner.SetActive(false);


            if (webRequest.isNetworkError)
            {
                UnityEngine.Debug.Log("Error: " + webRequest.error);
                errorHolder.SetActive(true);
                error.text = "Erro de rede!";
                ui.SetActive(true);
                yield return new WaitForSeconds(2);
                error.text = "";
                errorHolder.SetActive(false);
            }
            else if (webRequest.isHttpError)
            {
                errorHolder.SetActive(true);
                error.text = "Erro a traduzir!";
                ui.SetActive(true);
                yield return new WaitForSeconds(2);
                error.text = "";
                errorHolder.SetActive(false);
            }
            else
            {
                UnityEngine.Debug.Log("Received: " + webRequest.downloadHandler.text);
                String result = webRequest.downloadHandler.text;
                List<string> temp = result.Split(null).ToList();
                temp.RemoveAt(temp.Count - 1);
                temp.RemoveAt(temp.Count - 1);
                temp.RemoveAt(temp.Count - 1);
                temp.ForEach(x => { sequence.Enqueue(x); });
                StartCoroutine(ResulveSequence());
            }
        }
        isRequesting = false;
    }

    private IEnumerator ResulveSequence()
    {
        ui.SetActive(false);
        textfield.text = "";
        representationHolder.SetActive(true);
        float total = 4*100;
        for(int i = 0; sequence.Count != 0; i++)
        {
            float timer = 0;
            string name = sequence.Dequeue();
            UnityEngine.Debug.Log("PLAYING " + name.ToUpper());
            try
            {
                animator.Play(name.ToUpper());
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log("No animation data!");
                name += " (NO DATA)";
            }
            while (total >= timer)
            {
                representation.text = name.ToUpper();
                timer += animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                yield return null;
            }
            UnityEngine.Debug.Log(timer);
        }
        yield return new WaitForSeconds(1);
        animator.Play("Idle");
        textfield.text = "";
        representationHolder.SetActive(false);
        ui.SetActive(true);
    }
}
