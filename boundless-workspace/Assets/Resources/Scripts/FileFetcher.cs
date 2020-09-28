using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;

public class FileFetcher : MonoBehaviour
{
    private const string API_HOST = "damocles.cs.washington.edu";
    private const string API_PORT = "3000";

    void Start()
    {
        StartCoroutine(CheckTime());
    }

    IEnumerator CheckTime()
    {
        int i = 0;
        while (true)
        {
            string endpoint = String.Format("http://{0}:{1}/remove", API_HOST, API_PORT);
            using (UnityWebRequest request = UnityWebRequest.Post(endpoint, ""))
            {
                LogMessage(string.Format("About to send request #{0}.", i));

                yield return request.SendWebRequest();

                LogMessage(string.Format("Got response to request #{0}.", i));

                if (request.isNetworkError || request.isHttpError)
                {
                    LogMessage(string.Format("Response #{0}: ERROR\n{1}", i, request.error));
                }
                else
                {
                    string responseBody = request.downloadHandler.text;
                    LogMessage(string.Format("About to deserialize JSON of response #{0}.", i));
                    RemoveResponse rr = JsonConvert.DeserializeObject<RemoveResponse>(responseBody);
                    if (rr.next != null)
                    {
                        RemoveResponse.FileData f = rr.next.Value;
                        byte[] data = Convert.FromBase64String(f.data);
                        if (f.type == "jpg" || f.type == "png")
                        {
                            LogMessage(string.Format("B - Response #{0} (image)", i));

                            // These dimensions are random and won't have any influence; when we load in images, the
                            // dimensions will be overriden with the correct values.
                            Texture2D tex = new Texture2D(400, 400);
                            ImageConversion.LoadImage(tex, data);
                            float aspectRatio = (float)tex.width / tex.height;

                            float height = 0.1f;
                            float width = height * aspectRatio;

                            WindowController controller = WindowController.New2DWindow(width, height);
                            controller.SetTexture(tex);
                        }
                        else if (f.type == "txt")
                        {
                            string decodedString = Encoding.UTF8.GetString(data);
                            LogMessage(string.Format("B - Response #{0} (text)\n{1}", i, decodedString));

                            //Texture2D tex = new Texture2D(17, 22);

                            float METERS_PER_INCH = 0.0254f;

                            WindowController controller = WindowController.New2DWindow(8.5f * METERS_PER_INCH, 11.0f * METERS_PER_INCH, margin: 0.5f * METERS_PER_INCH);
                            //controller.SetTexture(tex);
                            controller.SetText(decodedString);
                        }
                        else if (f.type == "asset")
                        {
                            //for model
                            try
                            {
                                AssetBundle loadedAssetBundle = AssetBundle.LoadFromMemory(data);
                                if (loadedAssetBundle != null)
                                {
                                    WindowController controller = WindowController.New3DWindow();
                                    controller.SetModel(loadedAssetBundle);
                                }
                            } catch (Exception e)
                            {

                            }
                            finally
                            {

                            }
                        }
                        else
                        {
                            LogMessage(string.Format("C - Response #{0}: \n{1}", i, responseBody));
                        }
                    } else
                    {
                        LogMessage(string.Format("D - Response #{0}: \n{1}", i, responseBody));
                    }

                }
            }

            yield return new WaitForSeconds(1.0f);
            i = i + 1;
        }
    }

    void LogMessage(string message)
    {
        // Debug.Log(message);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

[System.Serializable]
struct RemoveResponse
{
    [System.Serializable]
    public struct FileData
    {
        public string path;
        public string type;
        public string data;
    }

    public FileData? next;
}

