using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class PostData : MonoBehaviour
{
	string serverUrl = "http://localhost:3000/sentdatatodb";
	PlayerData player;

	public void SetupPlayerData(string screenname, string firstname, string lastname, string date, int score)
	{
		player = new PlayerData();

		player.screenName = screenname;
		player.firstName = firstname;
		player.lastName = lastname;
		player.dateStartPlaying = date;
		player.score = score;

		string json = JsonUtility.ToJson(player);
		StartCoroutine(PostPlayerData(json));
	}

	IEnumerator PostPlayerData(string json)
	{
		byte[] jsonToSend = Encoding.UTF8.GetBytes(json);
		UnityWebRequest request = new UnityWebRequest(serverUrl, "POST");
		request.uploadHandler = new UploadHandlerRaw(jsonToSend);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");

		yield return request.SendWebRequest();

		if (request.result == UnityWebRequest.Result.Success)
		{
			string response = request.downloadHandler.text;
			string newPlayerId = ExtractPlayerId(response);
		}
		else Debug.LogError($"Error sending data: {request.error}");
	}

	string ExtractPlayerId(string jsonResponse)
	{
		int index = jsonResponse.IndexOf("\"playerid\":\"") + 12;
		if (index < 12) return "";
		int endIndex = jsonResponse.IndexOf("\"", index);
		return jsonResponse.Substring(index, endIndex - index);

	}
}