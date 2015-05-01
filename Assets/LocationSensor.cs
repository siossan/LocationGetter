using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LocationSensor : MonoBehaviour {
	/// <summary>フォント</summary>
	private GUIStyle labelStyle;

	/// <summary>位置情報取得フラグ</summary>
	private bool isGetLocation = false;
	/// <summary>緯度</summary>
	private float latitude;
	/// <summary>経度</summary>
	private float longitude;
	/// <summary>標高</summary>
	private float altitude;
	/// <summary>水平精度</summary>
	private float horizontalAccuracy;
	/// <summary>垂直精度</summary>
	private float verticalAccuracy;
	/// <summary>タイムスタンプ</summary>
	private double timestamp;

	// Use this for initialization
	IEnumerator Start() {
		//フォント生成
		this.labelStyle = new GUIStyle();
		this.labelStyle.fontSize = 12;
		this.labelStyle.normal.textColor = Color.white;

		if (!Input.location.isEnabledByUser) {
			yield break;
		}
		Input.location.Start();
		int maxWait = 15;
		while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
			yield return new WaitForSeconds(1);
			maxWait--;
		}
		if (maxWait < 1) {
			print("Timed out");
			yield break;
		}
		if (Input.location.status == LocationServiceStatus.Failed) {
			print("Unable to determine device location");
			yield break;
		} else {
			latitude = Input.location.lastData.latitude;
			longitude = Input.location.lastData.longitude;
			altitude = Input.location.lastData.altitude;
			horizontalAccuracy = Input.location.lastData.horizontalAccuracy;
			verticalAccuracy = Input.location.lastData.verticalAccuracy;
			timestamp = Input.location.lastData.timestamp;
			//print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
			isGetLocation = true;
		}
		Input.location.Stop();
	}

	// Update is called once per frame
	void Update() {
	}

	/// <summary>
	/// GUI更新
	/// </summary>
	void OnGUI() {
		float x = Screen.width / 10;
		float y = 0;
		float w = Screen.width * 8 / 10;
		float h = Screen.height / 20;

		for (int i = 0; i < 3; i++) {
			y = Screen.height / 10 + h * i;
			string text = string.Empty;

			switch (i) {
				case 0://latitude
					float dispLatitude = 0;
					if (isGetLocation) {
						dispLatitude = this.latitude;
					}
					text = string.Format("緯度:{0}", System.Math.Round(dispLatitude, 4));
					break;
				case 1://longitude
					float dispLongitude = 0;
					if (isGetLocation) {
						dispLongitude = this.longitude;
					}
					text = string.Format("緯度:{0}", System.Math.Round(dispLongitude, 4));
					break;
				case 2://longitude
					float dispAltitude = 0;
					if (isGetLocation) {
						dispAltitude = this.altitude;
					}
					text = string.Format("標高:{0}", System.Math.Round(dispAltitude, 4));
					break;
				default:
					throw new System.InvalidOperationException();
			}
			GUI.Label(new Rect(x, y, w, h), text, this.labelStyle);
		}
	}
}