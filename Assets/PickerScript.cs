using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameAnalyticsSDK;

public class PickerScript : MonoBehaviour
{
    private GameObject[] icons;
    private VectorObjectList positions = new VectorObjectList();
    private int numOfPlayers = 0;
    public GameObject icon1;
    public GameObject icon2;
    public GameObject icon3;
    public GameObject icon4;
    public GameObject icon5;
    public GameObject icon6;
    public GameObject icon7;
    public GameObject icon8;
    public GameObject icon9;
    public GameObject icon10;
    public GameObject BackButton;
    private bool gameActive = true;
    public Text Countdown;
    private float timeLeft = 5.0f;
    void Awake()
    {
        Debug.Log("Good Morning");
        icons = new GameObject[10]{icon1, icon2, icon3, icon4, icon5, icon6, icon7, icon8, icon9, icon10};
        GameAnalytics.Initialize();
    }
    void Update()
    {
        int touchCount = Input.touchCount;
        if(touchCount > 0 && gameActive && touchCount <= icons.Length)
        {
            if(touchCount == 2) BackButton.SetActive(false);
            for(int i = 0; i < touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                Vector2 pos = touch.position;
                if (touch.phase == TouchPhase.Began)
                {
                    createIcon(pos);
                }
                if (touch.phase == TouchPhase.Moved){
                    positions.UpdateWhereVector(pos);
                }
                if (touch.phase == TouchPhase.Ended){
                    destroyIcon(pos);
                }
            }
            if(touchCount > 1 && positions.GetLength() > 1)
            {
                if(numOfPlayers != 0 && numOfPlayers == touchCount)
                {
                    timeLeft -= Time.deltaTime;
                    if(timeLeft < 0)
                    {
                        Countdown.text = "";
                        this.PickRandom();
                    }else{
                        Countdown.text = timeLeft.ToString();
                    }
                }else{
                    numOfPlayers = touchCount;
                    Countdown.text = "5";
                    timeLeft = 5.0f;
                }
            }else{
                numOfPlayers = 0;
                Countdown.text = "";
            }
        }else if(touchCount == 0){
            if(positions.GetLength() > 0){
                positions.CleanScreen();
            }
            Countdown.text = "";
            gameActive = true;
            BackButton.SetActive(true);
        }
    }

    private void createIcon (Vector2 mousePos) {
        if(positions.GetLength() < icons.Length){
            int available = positions.GetAvailableIconIndex(icons);
            if(available != -1){
                icons[available].SetActive(true);
                icons[available].transform.position = new Vector3(mousePos.x,mousePos.y, 0);
                positions.Insert(new VectorObject(icons[available].transform.position, icons[available], available));
            }
        }
    }
    
    private void destroyIcon (Vector2 mousePos) {
        positions.RemoveWhereVector(mousePos);
    }

    private void PickRandom()
    {
        gameActive = false;
        int playerLength = positions.GetLength();
        System.Random r = new System.Random();
        int randomIndex = r.Next(playerLength); 
        for(int i = 0; i < positions.GetLength(); i++)
        { 
            Debug.Log( i + "random " + randomIndex);
            if(i < randomIndex){
                positions.RemoveAt(i);
                randomIndex--;
                i--;
            }
            if(i > randomIndex)
            {
                positions.RemoveAt(i);
                i--;
            }
        }
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "Play Game");
    }
}

public class VectorObject{
    private Vector3 position;
    public GameObject icon;
    public int iconIndex;
    public VectorObject(Vector3 v, GameObject obj, int index)
    {
        position = new Vector3(v.x, v.y, 0);
        icon = obj;
        iconIndex = index;
    }
    public float GetSquaredDistanceFromVector(Vector3 v){
        float x = System.Math.Abs(position.x - v.x);
        float y = System.Math.Abs(position.y - v.y);
        return x*x + y*y;
    }
    public void UpdatePosition(Vector2 mousePos){
        position = new Vector3(mousePos.x, mousePos.y, 0);
        icon.transform.position = position;
    }
}

public class VectorObjectList {
    private List<VectorObject> arr;
    public VectorObjectList(){
        arr = new List<VectorObject>();
    }
    private int ClosestToVector(Vector2 v){
        float smallest = -1;
        int index = -1;
        for(int i = 0; i < arr.Count; i ++){
            float distance = arr[i].GetSquaredDistanceFromVector(v);
            if(i == 0){
                smallest = distance;
                index = i;
            }else if(distance <= smallest){
                smallest = distance;
                index = i;
            }
        }
        return index;
    }
    public int GetAvailableIconIndex(GameObject[] icons){
        Dictionary<int, bool> occuppiedIcons = new Dictionary<int, bool>();
        for(int i = 0; i < arr.Count; i++){
            occuppiedIcons.Add(arr[i].iconIndex, true);
        }
        for(int i = 0; i < icons.Length; i++){
            if(!occuppiedIcons.ContainsKey(i)){
                return i;
            }
        }
        return -1;
    }
    public int GetLength(){
        return arr.Count;
    }
    public void UpdateWhereVector(Vector2 v){
        int index = this.ClosestToVector(v);
        if(index != -1)
        {
            arr[index].UpdatePosition(v);
        }
    }
    public void RemoveWhereVector(Vector2 v){
        int index = this.ClosestToVector(v);
        if(index != -1)
        {
            arr[index].icon.SetActive(false);
            arr.RemoveAt(index);
        }
    }
    public void CleanScreen()
    {
        for (int i = 0; i < arr.Count; i++)
        {
            arr[0].icon.SetActive(false);
            arr.RemoveAt(0);
        }
    }
    public void RemoveAt(int index){
        arr[index].icon.SetActive(false);
        arr.RemoveAt(index);
    }
    public void Insert(VectorObject obj){
        arr.Add(obj);
    }
}
