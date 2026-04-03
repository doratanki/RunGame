#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Encoder;
using UnityEditor.Recorder.Input;
using UnityEngine;

/// <summary>
/// エディター専用の録画ウィンドウ。
/// メニュー: Tools > Game Recorder
/// </summary>
public class GameRecorderWindow : EditorWindow
{
    private RecorderController _controller;
    private RecorderControllerSettings _controllerSettings;
    private bool _isRecording = false;

    private string _outputPath = "Recordings";
    private string _fileName   = "StackTower";
    private int    _frameRate  = 60;
    private int    _width      = 1080;
    private int    _height     = 1920;

    [MenuItem("Tools/Game Recorder")]
    public static void ShowWindow()
    {
        GetWindow<GameRecorderWindow>("Game Recorder");
    }

    void OnGUI()
    {
        GUILayout.Label("録画設定", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        _outputPath = EditorGUILayout.TextField("保存フォルダ", _outputPath);
        _fileName   = EditorGUILayout.TextField("ファイル名",   _fileName);
        _frameRate  = EditorGUILayout.IntField("フレームレート", _frameRate);
        _width      = EditorGUILayout.IntField("幅",           _width);
        _height     = EditorGUILayout.IntField("高さ",         _height);

        EditorGUILayout.Space();

        if (!_isRecording)
        {
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("● 録画開始", GUILayout.Height(40)))
                StartRecording();
        }
        else
        {
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("■ 録画停止", GUILayout.Height(40)))
                StopRecording();
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            _isRecording ? "録画中... Play モードで実行してください。" : "Play モードを開始してから録画ボタンを押してください。",
            _isRecording ? MessageType.Warning : MessageType.Info);

        if (!string.IsNullOrEmpty(_outputPath))
        {
            string fullPath = Path.Combine(Application.dataPath, "..", _outputPath);
            EditorGUILayout.LabelField("保存先:", Path.GetFullPath(fullPath));
        }
    }

    void StartRecording()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorUtility.DisplayDialog("Game Recorder", "Play モードを開始してから録画してください。", "OK");
            return;
        }

        _controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
        _controller = new RecorderController(_controllerSettings);

        var videoSettings = ScriptableObject.CreateInstance<MovieRecorderSettings>();
        videoSettings.name            = "MP4 Recorder";
        videoSettings.Enabled         = true;
        videoSettings.EncoderSettings = new CoreEncoderSettings
        {
            EncodingQuality = CoreEncoderSettings.VideoEncodingQuality.High,
            Codec = CoreEncoderSettings.OutputCodec.MP4
        };
        videoSettings.ImageInputSettings = new GameViewInputSettings
        {
            OutputWidth  = _width,
            OutputHeight = _height
        };

        string dir = Path.Combine(Application.dataPath, "..", _outputPath);
        Directory.CreateDirectory(dir);
        videoSettings.OutputFile = Path.Combine(dir, _fileName + "_<Take>");

        _controllerSettings.FrameRate    = _frameRate;
        _controllerSettings.CapFrameRate = true;
        _controllerSettings.AddRecorderSettings(videoSettings);

        _controller.PrepareRecording();
        _controller.StartRecording();
        _isRecording = true;

        Debug.Log("[GameRecorder] 録画開始");
        Repaint();
    }

    void StopRecording()
    {
        if (_controller != null && _isRecording)
        {
            _controller.StopRecording();
            _isRecording = false;
            Debug.Log("[GameRecorder] 録画停止 → " + _outputPath);
            Repaint();
        }
    }

    void OnDestroy()
    {
        StopRecording();
    }
}
#endif
