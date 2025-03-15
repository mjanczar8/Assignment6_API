using TMPro;
using UnityEngine;


public class FindPlayer : MonoBehaviour
{
	public TMP_InputField name;
	public FetchData fetch;

	public void SearchForPlayer()
	{
		if (name.text != "")
			fetch.SetupPlayerSearchData(name.text);
	}
}