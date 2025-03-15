using TMPro;
using UnityEngine;

public class SendPlayer : MonoBehaviour
{
	public TMP_InputField screenName;
	public TMP_InputField fName;
	public TMP_InputField lName;
	public TMP_InputField date;
	public TMP_InputField score;
	public PostData post;

	public void SendData()
	{
		if (screenName.text != "" && fName.text != "" && score.text != "" && lName.text != "" && date.text != "")
		{
			int scoreData = int.Parse(score.text);
			post.SetupPlayerData(screenName.text, fName.text, lName.text, date.text, scoreData);
		}
	}
}