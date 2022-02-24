using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO ; 

public class PROUVE_audioRecorder : MonoBehaviour
{
    public string recordingDevice ; 
    private bool isRecording = false ; 
    private int defaultRecordingTime = 1 ; 
    private List <AudioClip> audioRecord = new List<AudioClip>() ; 
    //private AudioSource audioSource ; 

    // Start is called before the first frame update
    void Start()
    {
    //    audioSource = GetComponent<AudioSource>() ; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setRecordingDevice(string device) {
        recordingDevice = device ; 
    }

    public void StartRecording() {
        isRecording = true ; 
        StartCoroutine(recordingMechanic()) ; 
    }

    public void StopRecording() {
        isRecording = false ; 
        Microphone.End(recordingDevice) ; 
    }

    public void Play() {
        for (int i=0; i< audioRecord.Count; i++) {
                Debug.Log("Something") ; 
        }
    }

    public void Save(string filename) {
        AudioClip combined_clip = Combine(audioRecord.ToArray());
        SavWav.Save(filename,combined_clip);
    }

    public void Save_deprecated(string filename) {


        FileStream fsWrite = File.Open(filename, FileMode.Create);
 
        BinaryWriter bw = new BinaryWriter(fsWrite);
 
        byte[] header = { 82, 73, 70, 70, 22, 10, 4, 0, 87, 65, 86, 69, 102, 109, 116, 32 };
        bw.Write(header);
 
        byte[] header2 = { 16, 0, 0, 0, 1, 0, 1, 0, 68, 172, 0, 0, 136, 88, 1, 0 };
        bw.Write(header2);
 
        byte[] header3 = { 2, 0, 16, 0, 100, 97, 116, 97, 152, 9, 4, 0 };
        bw.Write(header3);

        int samplesCount = 0 ; 
        float[] totalSamples = new float[0];  
        Debug.Log("Number of audio clips : "+audioRecord.Count) ; 
        Debug.Log("Theorical length of audio clip : "+ 44100*defaultRecordingTime) ; 
        Debug.Log("Length of first audio clip : "+audioRecord[0].samples) ; 
        for(int j=0; j < audioRecord.Count ; j++) {
            samplesCount += audioRecord[j].samples ; 
            float[] samples = new float[audioRecord[j].samples];
            audioRecord[j].GetData(samples,0) ; 
            int longueur = totalSamples.Length + samples.Length ; 
            var z = new float[longueur] ; 
            totalSamples.CopyTo(z,0) ; 
            samples.CopyTo(z,totalSamples.Length) ; 
            totalSamples = z ; 
        }
        Debug.Log("Length of all the samples : "+totalSamples.Length) ; 
       // float[] samples = new float[audioClip.samples];
       // audioClip.GetData(samples, 0);

        int i = 0;
 
        while (i < totalSamples.Length)
        {
            int sampleInt = (int)(32000.0 * totalSamples[i++]);
 
            int msb = sampleInt / 256;
            int lsb = sampleInt - (msb * 256);
 
            bw.Write((byte)lsb);
            bw.Write((byte)msb);
        }
 
        fsWrite.Close();
    }

    IEnumerator recordingMechanic() {
        //AudioSource audioSource = GetComponent<AudioSource>();
        AudioClip monClip = Microphone.Start(recordingDevice, false, defaultRecordingTime, 44100);
        yield return new WaitForSeconds(defaultRecordingTime) ; 
        Microphone.End(recordingDevice) ; 
        audioRecord.Add(monClip) ; 
        if(isRecording) {StartCoroutine(recordingMechanic()); }
    }

    public static AudioClip Combine(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0)
            return null;
    
        int length = 0;
        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i] == null)
                continue;
    
            length += clips[i].samples * clips[i].channels;
        }
    
        float[] data = new float[length];
        length = 0;
        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i] == null)
                continue;
    
            float[] buffer = new float[clips[i].samples * clips[i].channels];
            clips[i].GetData(buffer, 0);
            //System.Buffer.BlockCopy(buffer, 0, data, length, buffer.Length);
            buffer.CopyTo(data, length);
            length += buffer.Length;
        }
    
        if (length == 0)
            return null;
    
        AudioClip result = AudioClip.Create("Combine", length, 1, 44100, false);
        result.SetData(data, 0);
    
        return result;
    }


}
