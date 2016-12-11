using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;
using Pomelo.DotNetClient;
using UnityEngine.UI;

using LitJson;


public class ChatGUI : MonoBehaviour
{
    // 直接从login 获取users和PomeloClient
    private string userName = LoginGUI.userName;
    private PomeloClient pclient = LoginGUI.pomeloClient;


    private Vector3 chatScrollPosition;
    private Vector3 userScrollPosition;

    private ArrayList userlist = null; // 存储所有用户
    private ArrayList userGameObject = null;
    private ArrayList moveRecords = null;

    void Start()
    {
        Application.runInBackground = true;

        userlist = new ArrayList();
        userGameObject = new ArrayList();
        moveRecords = new ArrayList();

        // 
        InitUserWindow();

        pclient.on("onAdd", (data) =>
        {
            RefreshUserWindow("add", data);
        });

        pclient.on("onLeave", (data) =>
        {
            RefreshUserWindow("leave", data);
        });

        //pclient.on("onChat", (data) =>
        //{
        //    addMessage(data);
        //});

        pclient.on("onMove", (data) =>
        {
            addMoveMessage(data);
        });
    }

    void addMoveMessage(JsonObject messge)
    {
        string s = messge.ToString();
        JsonData jd = JsonMapper.ToObject(s);
        string _nanme = jd["from"].ToString();
        int _posx = int.Parse(jd["posx"].ToString());
        int _posy = int.Parse(jd["posy"].ToString());
        moveRecords.Add(new MoveRecord(_nanme, _posx, _posy));
    }

    //Update the userlist.
    void RefreshUserWindow(string flag, JsonObject msg)
    {

        if (flag == "add")
        {
            string s = msg.ToString();
            JsonData jd = JsonMapper.ToObject(s);

            Player p = new Player();
            p.name = jd["user"].ToString();
            p.posx = int.Parse(jd["posx"].ToString());
            p.posy = int.Parse(jd["posy"].ToString());
            userlist.Add(p);
        }
        else if (flag == "leave")
        {
            string s = msg.ToString();
            JsonData jd = JsonMapper.ToObject(s);
            string name = jd["user"].ToString(); // del name

            int index = -1;
            for (int i = 0; i < userlist.Count; i++)
            {
                if (((Player)(userlist[i])).name == name)
                {
                    index = i;
                    break;
                }
            }
            if (index != -1)
                userlist.RemoveAt(index);

        }
    }


    //When quit, release resource
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape) || Input.GetKey("escape"))
        {
            if (pclient != null)
            {
                pclient.disconnect();
            }
            Application.Quit();
        }

        //
        foreach (GameObject go in userGameObject)
        {
            Destroy(go);
        }
        userGameObject.Clear();

        if (userName != null && userName != "")
        {
            foreach (Player p in userlist)
            {
                Vector3 v3 = new Vector3(p.posx, p.posy, 0);
                GameObject mon;
                mon = (GameObject)Instantiate(Resources.Load("ME"), v3, Quaternion.identity);
                mon.GetComponent<TextMesh>().text = p.name;
                if (p.name == userName)
                    mon.GetComponent<TextMesh>().color = Color.red;
                else
                    mon.GetComponent<TextMesh>().color = Color.blue;
                mon.transform.position = v3;

                userGameObject.Add(mon);
            }
        }

        // 遍历动作
        foreach(MoveRecord mr in moveRecords)
        {
            Debug.Log("make a move");

            foreach (GameObject go2 in userGameObject)
            {
                if (go2.GetComponent<TextMesh>().text == mr.name)
                {
                    float walkSpeed = 2f;
                    Vector3 v3 = new Vector3(mr.new_posx, mr.new_posy, 0);
                    Vector3 offSet = v3 - go2.transform.position;

                    //if(Vector3.Distance(v3, go2.transform.position) > 0.02f)
                    //    go2.transform.position += walkSpeed * offSet.normalized * Time.deltaTime;
                    go2.transform.position = v3;

                    //int len = userlist.Count;
                    //for (int i = 0; i < len; i++)
                    //{
                    //    if (((Player)userlist[i]).name == mr.name) {
                    //        ((Player)userlist[i]).posx = mr.new_posx;
                    //        ((Player)userlist[i]).posx = mr.new_posy;
                    //    }
                    //}
                }
            }
            // 更新userList集合
            foreach (Player p in userlist)
            {
                if (p.name == mr.name)
                {
                    p.posx = mr.new_posx;
                    p.posy = mr.new_posy;
                }
            }
        }
        moveRecords.Clear();

        // 按A键
        if (Input.GetKeyDown(KeyCode.A))
        {
            
            Vector3 pos = Input.mousePosition;
            
            foreach (GameObject go2 in userGameObject)
            {
                if (go2.GetComponent<TextMesh>().text == userName)
                {
                    //float walkSpeed = 2f;
                    // float deltaX = pos.x - go2.transform.position.x;
                    // float deltaY = pos.y - go2.transform.position.y;
                    //Vector3 v3 = new Vector3(go2.transform.position.x - 1, go2.transform.position.y, 0);
                    //go2.transform.position = transform.position + walkSpeed * v3 * Time.deltaTime;

                    JsonObject message = new JsonObject();
                    message.Add("rid", LoginGUI.channel);
                    message.Add("posx", (int)go2.transform.position.x - 1);
                    message.Add("posy", (int)go2.transform.position.y);
                    message.Add("from", LoginGUI.userName);
                    message.Add("target", "*");
                    pclient.request("chat.chatHandler.move", message, (data) =>
                    {
                        Debug.Log(data.ToString());
                       // moveRecords.Add(new MoveRecord(userName, (int)go2.transform.position.x - 1, (int)go2.transform.position.y));
                    });

                }
            }

        }

        // D键
        if (Input.GetKeyDown(KeyCode.D))
        {

            Vector3 pos = Input.mousePosition;

            foreach (GameObject go2 in userGameObject)
            {
                if (go2.GetComponent<TextMesh>().text == userName)
                {
                    float walkSpeed = 2f;
                    // float deltaX = pos.x - go2.transform.position.x;
                    // float deltaY = pos.y - go2.transform.position.y;
                    Vector3 v3 = new Vector3(go2.transform.position.x + 1, go2.transform.position.y, 0);
                    go2.transform.position = transform.position + walkSpeed * v3 * Time.deltaTime;
                }
            }

            foreach (Player p in userlist)
            {
                if (p.name == userName)
                {
                    p.posx = p.posx + 1;
                }
            }
        }


    }

    //When quit, release resource
    void OnApplicationQuit()
    {
        if (pclient != null)
        {
            pclient.disconnect();
        }
    }

    // Init userList and userWindow
    void InitUserWindow()
    {
        JsonObject jsonObject = LoginGUI.users;

        string s = jsonObject.ToString();

        JsonData jd = JsonMapper.ToObject(s);
        int len = jd["users"].Count;

        ICollection<string> it = jd["users"].Keys;
        foreach (string c in it)
        {
            string uid = c;
            string ss = jd["users"][uid].ToJson();

            Player b = JsonMapper.ToObject<Player>(ss);
            userlist.Add(b);
        }
    }

}