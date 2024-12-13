using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;

public class LidarPublisher : MonoBehaviour
{
    ROSConnection ros;
    float time = 0.0f;

    public int rayCount = 100;               // レイの本数
    public float angleRange = 270f;          // レイの角度範囲
    public float rayDistance = 8f;          // レイの最大距離
    public float hz = 15.0f;                 // パブリッシュ周波数
    public string topicName = "/scan"; // トピック名

    void Start()
    {
        // ROSコネクションの取得
        ros = ROSConnection.GetOrCreateInstance();
        
        // パブリッシャの登録
        ros.RegisterPublisher<LaserScanMsg>(topicName);
    }

    void Update()
    {
        time += Time.deltaTime;
        if (time < 1/hz) // 毎秒パブリッシュ
        {
            return;
        }
        time = 0.0f;

        // LaserScanメッセージの準備
        LaserScanMsg laserScanMsg = new LaserScanMsg
        {
            header = new RosMessageTypes.Std.HeaderMsg
            {
                frame_id = "map" // frame_idを"map"に設定
            },
            angle_min = -angleRange / 2 * Mathf.Deg2Rad,  // 左端の角度（ラジアン）
            angle_max = angleRange / 2 * Mathf.Deg2Rad,   // 右端の角度（ラジアン）
            angle_increment = (angleRange / (rayCount - 1)) * Mathf.Deg2Rad, // 角度ステップ（ラジアン）
            range_min = 0.1f,                             // 最小範囲
            range_max = rayDistance                       // 最大範囲
        };

        // 各レイの距離を格納するリスト
        List<float> ranges = new List<float>();

        float angleStep = angleRange / (rayCount - 1);
        float startAngle = angleRange / 2;

        for (int i = 0; i < rayCount; i++)
        {
            float currentAngle = startAngle - (angleStep * i);
            Vector3 direction = Quaternion.Euler(0, currentAngle, 0) * transform.forward;

            Ray ray = new Ray(transform.position, direction);
            RaycastHit hit;

            // レイキャストで障害物の距離を取得
            if (Physics.Raycast(ray, out hit, rayDistance))
            {
                ranges.Add(hit.distance);  // 障害物までの距離を追加
                Debug.DrawLine(transform.position, hit.point, Color.red); // 障害物があった場合は赤色
            }
            else
            {
                ranges.Add(rayDistance); // 障害物がない場合は最大距離
                Debug.DrawLine(transform.position, transform.position + direction * rayDistance, Color.green); // 障害物がない場合は緑色
            }
        }

        // rangesリストをLaserScanメッセージに設定
        laserScanMsg.ranges = ranges.ToArray();

        // LaserScanメッセージをパブリッシュ
        ros.Publish(topicName, laserScanMsg);
    }
}
