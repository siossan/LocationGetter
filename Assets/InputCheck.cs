using UnityEngine;
using System.Collections;
using System;

public class InputCheck : MonoBehaviour {

	/// <summary>加速度センサーの値</summary>
	private Vector3 _acceleration;
	/// <summary>フォント</summary>
	private GUIStyle _labelStyle;

	/// <summary>位置情報取得フラグ</summary>
	private bool _isGetLocation = false;
	/// <summary>緯度</summary>
	private float _latitude;
	/// <summary>経度</summary>
	private float _longitude;
	/// <summary>標高</summary>
	private float _altitude;
	/// <summary>水平精度</summary>
	private float _horizontalAccuracy;
	/// <summary>垂直精度</summary>
	private float _verticalAccuracy;
	/// <summary>タイムスタンプ</summary>
	private double _timestamp;

	/// <summary>最初の更新</summary>
	private bool _isFirstUpdate = true;
	/// <summary>前回の緯度</summary>
	private float _latitudePrev = 0.0f;
	/// <summary>前回の経度</summary>
	private float _longitudePrev = 0.0f;
	/// <summary>前回と今回の距離</summary>
	private float _distance;

	/// <summary>位置情報の取得精度</summary>
	private static readonly float DESIRED_ACCURACY_IN_METERS = 5.0f;
	/// <summary>位置情報の更新間隔</summary>
	private static readonly float UPDATE_DISTANCE_IN_METERS = 1.0f;
	/// <summary>経緯度の丸め位置</summary>
	private static readonly int LOCATION_SCALE = 10;

	/// <summary>
	// Use this for initialization.
	/// </summary>
	IEnumerator Start() {

		// sleepOff
		Screen.sleepTimeout = SleepTimeout.NeverSleep;

		// Timmer set
		startTime = Time.time;

		//フォント生成
		this._labelStyle = new GUIStyle();
		this._labelStyle.fontSize = Screen.height / 22;
		this._labelStyle.normal.textColor = Color.black;
		Input.compass.enabled = true;

		// 位置情報サービスを利用できるか判定
		if (!Input.location.isEnabledByUser) {
			yield break;
		}

		// 位置情報サービスをスタート
		Input.location.Start(DESIRED_ACCURACY_IN_METERS, UPDATE_DISTANCE_IN_METERS);

		// 位置情報サービスの初期化
		StartWait();

		// 継続して位置情報を取得したいのでＳＴＯＰしない
		// Input.location.Stop();
	}

	/// <summary>
	/// 位置情報サービスを初期化します<br>
	/// </summary>
	/// <returns>初期化している最中にアプリに通知される待機時間(秒)</returns>
	IEnumerator StartWait() {
		int maxWait = 15;
		while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
			yield return new WaitForSeconds(1);
			maxWait--;
		}
		if (maxWait < 1) {
			print("Timed out");
			yield break;
		}
	}

	// Update is called once per frame
	void Update() {
		//文字描画はOnGUIでしかできないらしいので保持
		this._acceleration = Input.acceleration;

		// update location
		if (Input.location.status == LocationServiceStatus.Failed) {
			print("Unable to determine device location");

		} else {
			_latitude = Input.location.lastData.latitude;
			_longitude = Input.location.lastData.longitude;
			_altitude = Input.location.lastData.altitude;
			_horizontalAccuracy = Input.location.lastData.horizontalAccuracy;
			_verticalAccuracy = Input.location.lastData.verticalAccuracy;
			_timestamp = Input.location.lastData.timestamp;
			//print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
			_isGetLocation = true;

			if (_isFirstUpdate) {
				_distance = 0;
				_isFirstUpdate = false;

			} else {
				// 座標に変更があった場合は距離を計算する
				if (_latitude != _latitudePrev || _longitude != _longitudePrev) {
					_distance = GetDistance(_latitude, _longitude, _latitudePrev, _longitudePrev, 9);
				}
			}

			// 前回情報の更新
			_latitudePrev = _latitude;
			_longitudePrev = _longitude;


			if(startTime - Time.time % 5 == 0){

				string url = "http://www.snowwhite.hokkaido.jp/niseko/api";
				WWWForm form = new WWWForm();
				form.AddField("lon", "fuga");
				form.AddField("lat", "bar");
				form.AddField("alt", "fuga");
				form.AddField("acc", "bar");
				form.AddField("speed", "fuga");
				form.AddField("provider", "bar");
				form.AddField("gps_status", "fuga");
				form.AddField("r_datetime", "bar");
				form.AddField("mobile_location_id", "fuga");
				form.AddField("u_id", "bar");
				WWW www = new WWW(url, form);
				yield return www;
				if (www.error == null) {
					Debug.Log(www.text);
				}

			}
		}
	}

	/// <summary>
	/// GUI更新
	/// </summary>
	void OnGUI() {
		if (_acceleration != null) {
			float x = Screen.width / 10;
			float y = 0;
			float w = Screen.width * 8 / 10;
			float h = Screen.height / 20;

			for (int i = 0; i < 8; i++) {
				y = Screen.height / 10 + h * i;
				string text = string.Empty;

				switch (i) {
					case 0://X
						text = string.Format("accel-X:{0}", System.Math.Round(this._acceleration.x, 3));
						break;
					case 1://Y
						text = string.Format("accel-Y:{0}", System.Math.Round(this._acceleration.y, 3));
						break;
					case 2://Z
						text = string.Format("accel-Z:{0}", System.Math.Round(this._acceleration.z, 3));
						break;
					case 3://magneticHeading
						text = string.Format("magnetic-heading:{0}", (int)(Input.compass.magneticHeading));
						break;
					case 4://latitude
						float dispLatitude = 0;
						if (_isGetLocation) {
							dispLatitude = this._latitude;
						}
						text = string.Format("緯度:{0}", System.Math.Round(dispLatitude, LOCATION_SCALE));
						break;
					case 5://longitude
						float dispLongitude = 0;
						if (_isGetLocation) {
							dispLongitude = this._longitude;
						}
						text = string.Format("緯度:{0}", System.Math.Round(dispLongitude, LOCATION_SCALE));
						break;
					case 6://longitude
						float dispAltitude = 0;
						if (_isGetLocation) {
							dispAltitude = this._altitude;
						}
						text = string.Format("標高:{0}", System.Math.Round(dispAltitude, LOCATION_SCALE));
						break;
					case 7:
						float dispDistance = 0;
						if (_isGetLocation) {
							dispDistance = this._distance;
						}
						text = string.Format("移動距離:{0}m", System.Math.Round(dispDistance, LOCATION_SCALE));
						break;
					default:
						throw new System.InvalidOperationException();
				}

				GUI.Label(new Rect(x, y, w, h), text, this._labelStyle);
			}
		}
	}


	private static float GetDistance(double lat1, double lng1, double lat2, double lng2, int scale) {
		// 2点の緯度の平均
		double latAvg = deg2rad(lat1 + ((lat2 - lat1) / 2.0f));
		// 2点の緯度差
		double latDifference = deg2rad(lat1 - lat2);
		// 2点の経度差
		double lonDifference = deg2rad(lng1 - lng2);

		double curRadiusTemp = 1 - 0.00669438 * Math.Pow(Math.Sin(latAvg), 2);
		// 子午線曲率半径
		double meridianCurvatureRadius = 6335439.327 / Math.Sqrt(Math.Pow(curRadiusTemp, 3));
		// 卯酉線曲率半径
		double primeVerticalCircleCurvatureRadius = 6378137 / Math.Sqrt(curRadiusTemp);

		// 2点間の距離
		double distance = Math.Pow(meridianCurvatureRadius * latDifference, 2)
			+ Math.Pow(primeVerticalCircleCurvatureRadius
			* Math.Cos(latAvg) * lonDifference, 2);
		distance = Math.Sqrt(distance);

		return (float)Math.Round(distance, scale);
	}

	/// <summary>
	/// 度単位から等価なラジアン単位に変換します。
	/// </summary>
	/// <param name="deg">度単位</param>
	/// <returns></returns>
	private static double deg2rad(double deg) {
		return (deg / 180) * Math.PI;
	}
}