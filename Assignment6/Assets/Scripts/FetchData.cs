using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Text;

public class FetchData : MonoBehaviour
{
	string serverUrl = "http://localhost:3000/player";
	List<PlayerData> playerList;
	PlayerData player;
	public GameObject playerData;

	void Start()
	{
		StartFetch();
	}

	public IEnumerator GetData()
	{
		using (UnityWebRequest request = UnityWebRequest.Get(serverUrl))
		{
			yield return request.SendWebRequest();

			if (request.result == UnityWebRequest.Result.Success)
			{
				string json = request.downloadHandler.text;
				Debug.Log($"Recieved the data: {json}");

				playerList = JsonConvert.DeserializeObject<List<PlayerData>>(json);
			}
			else Debug.Log($"Error fetching data: {request.error}");
		}
	}

	public IEnumerator GetDataByName(string json, string playerName)
	{
		string url = serverUrl + "/" + playerName;
		byte[] jsonToSend = Encoding.UTF8.GetBytes(json);
		UnityWebRequest request = new UnityWebRequest(url, "GET");
		request.uploadHandler = new UploadHandlerRaw(jsonToSend);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");

		yield return request.SendWebRequest();

		if (request.result == UnityWebRequest.Result.Success)
		{
			string response = request.downloadHandler.text;
			Debug.Log($"Success: {response}");
			player = JsonConvert.DeserializeObject<PlayerData>(response);
		}
		else
		{
			Debug.Log("Error: " + request.error);
			yield return null;
		}
	}

	public void StartFetch()
	{
		StartCoroutine(GetData());
	}

	public void SetupPlayerSearchData(string username)
	{
		player = new PlayerData();
		player.screenName = username;
		string json = JsonUtility.ToJson(player);
		StartCoroutine(GetDataByName(json, username));
	}

	string ExtractPlayerId(string jsonResponse)
	{
		int index = jsonResponse.IndexOf("\"playerid\":\"") + 12;
		if (index < 12) return "";
		int endIndex = jsonResponse.IndexOf("\"", index);
		return jsonResponse.Substring(index, endIndex - index);
	}
}

public class PlayerData
{
	public string screenName;
	public string firstName;
	public string lastName;
	public string dateStartPlaying;
	public int score;
}