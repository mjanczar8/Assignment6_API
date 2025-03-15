using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class EditNDelete : MonoBehaviour
{
	public string serverUrl = "http://localhost:3000/player";
	private List<PlayerData> players = new List<PlayerData>();

	public GameObject playerItemPrefab;
	public Transform contentPanel;
	public GameObject editPanel;
	public TMP_InputField screenNameInput;
	public TMP_InputField firstNameInput;
	public TMP_InputField lastNameInput;
	public TMP_InputField dateInput;
	public TMP_InputField scoreInput;
	public Button saveButton;
	public Button cancelButton;
	private PlayerData selectedPlayer;

	void Start()
	{
		StartCoroutine(GetPlayers());
	}

	public void FetchAndDisplayPlayers()
	{
		StartCoroutine(GetPlayers());
	}

	private IEnumerator GetPlayers()
	{
		using (UnityWebRequest request = UnityWebRequest.Get(serverUrl))
		{
			request.SetRequestHeader("Content-Type", "application/json");
			yield return request.SendWebRequest();

			if (request.result == UnityWebRequest.Result.Success)
			{
				string json = request.downloadHandler.text;
				players = JsonConvert.DeserializeObject<List<PlayerData>>(json);
				players = players.OrderBy(p => p.screenName).ToList();

				DisplayPlayers();
			}
			else Debug.LogError("Error fetching players: " + request.error);
		}
	}

	private void DisplayPlayers()
	{
		foreach (Transform child in contentPanel)
		{
			Destroy(child.gameObject);
		}

		foreach (PlayerData player in players)
		{
			GameObject playerItem = Instantiate(playerItemPrefab, contentPanel);
			TMP_Text[] textFields = playerItem.GetComponentsInChildren<TMP_Text>();

			textFields[0].text = player.screenName;
			textFields[1].text = player.firstName;
			textFields[2].text = player.lastName;
			textFields[3].text = player.dateStartPlaying;
			textFields[4].text = player.score.ToString();

			Button[] buttons = playerItem.GetComponentsInChildren<Button>();
			PlayerData capturedPlayer = player;

			buttons[0].onClick.RemoveAllListeners();
			buttons[0].onClick.AddListener(() => OpenEditPanel(capturedPlayer));

			buttons[1].onClick.RemoveAllListeners();
			buttons[1].onClick.AddListener(() => DeletePlayer(capturedPlayer));
		}
	}

	private void OpenEditPanel(PlayerData player)
	{
		selectedPlayer = player;

		screenNameInput.text = player.screenName;
		firstNameInput.text = player.firstName;
		lastNameInput.text = player.lastName;
		dateInput.text = player.dateStartPlaying;
		scoreInput.text = player.score.ToString();

		editPanel.SetActive(true);
	}

	public void SavePlayerEdits()
	{
		if (selectedPlayer == null) return;

		selectedPlayer.firstName = firstNameInput.text;
		selectedPlayer.lastName = lastNameInput.text;
		selectedPlayer.dateStartPlaying = dateInput.text;
		selectedPlayer.score = int.Parse(scoreInput.text);

		string json = JsonConvert.SerializeObject(selectedPlayer);
		StartCoroutine(UpdatePlayerRequest(json, selectedPlayer.screenName));
	}

	private IEnumerator UpdatePlayerRequest(string json, string playerName)
	{
		string url = $"http://localhost:3000/update/{playerName}";


		using (UnityWebRequest request = new UnityWebRequest(url, "PUT"))
		{
			byte[] jsonToSend = System.Text.Encoding.UTF8.GetBytes(json);
			request.uploadHandler = new UploadHandlerRaw(jsonToSend);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");

			yield return request.SendWebRequest();

			if (request.result == UnityWebRequest.Result.Success)
			{
				Debug.Log($"Player updated: {playerName}");
				editPanel.SetActive(false);
				FetchAndDisplayPlayers(); // Refresh list
			}
			else Debug.LogError($"Error updating player: {request.error}");
		}
	}

	private void DeletePlayer(PlayerData player)
	{
		Debug.Log($"Deleting {player.screenName}");
		StartCoroutine(DeletePlayerRequest(player.screenName));
	}

	private IEnumerator DeletePlayerRequest(string playerName)
	{
		string url = $"http://localhost:3000/delete/{playerName}";
		using (UnityWebRequest request = UnityWebRequest.Delete(url))
		{
			yield return request.SendWebRequest();

			if (request.result == UnityWebRequest.Result.Success)
			{
				Debug.Log($"Player {playerName} deleted successfully.");
				FetchAndDisplayPlayers();
			}
			else Debug.LogError($"Error deleting player: {request.error}");
		}
	}

	public void CancelEdit()
	{
		editPanel.SetActive(false);
	}
}