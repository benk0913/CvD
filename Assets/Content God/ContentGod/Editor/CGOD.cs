
using UnityEditor;
using UnityEngine;

public class CGOD : EditorWindow
{

    CGODSubwindow CurrentSubwindow;

    [MenuItem("CvD/Content God")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        CGOD window = (CGOD)EditorWindow.GetWindow(typeof(CGOD));
        window.Show();
    }

    void OnGUI()
    {
        switch(CurrentSubwindow)
        {
            case CGODSubwindow.MainWindow:
                {
                    DrawMainWindow();
                    break;
                }
            case CGODSubwindow.Config:
                {
                    DrawConfigWindow();
                    break;
                }
            case CGODSubwindow.Help:
                {
                    DrawHelpWindow();
                    break;
                }

        }
    }

    void DrawMainWindow()
    {

        EditorGUILayout.HelpBox("Utilize this window to Create / Modify / Delete / Reshape the World!", MessageType.Info);

        EditorGUILayout.BeginVertical();



        //Header
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Config"))
        {
            CurrentSubwindow = CGODSubwindow.Config;
        }

        if (GUILayout.Button("Help"))
        {
            CurrentSubwindow = CGODSubwindow.Help;
        }

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("MASS UPDATE"))
        {

        }

        EditorGUILayout.EndHorizontal();

        //Sector Configurations
        GUI.skin.button.stretchWidth = true;
        GUI.skin.button.stretchHeight = true;


        //TOP ROW
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Items"))
        {

        }
        if (GUILayout.Button("Monsters"))
        {

        }
        if (GUILayout.Button("Races"))
        {

        }
        EditorGUILayout.EndHorizontal();

        //BOTTOM ROW
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Classes"))
        {

        }
        if (GUILayout.Button("Abilities"))
        {

        }
        if (GUILayout.Button("Buffs"))
        {

        }
        EditorGUILayout.EndHorizontal();


        //Sector DeConfigurations
        GUI.skin.button.stretchWidth = false;
        GUI.skin.button.stretchHeight = false;


        EditorGUILayout.EndVertical();
    }

    void DrawConfigWindow()
    {
        EditorGUILayout.HelpBox("Settings", MessageType.Info);

        GUILayout.BeginVertical();

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Back"))
        {
            CurrentSubwindow = CGODSubwindow.MainWindow;
        }

        GUILayout.EndVertical();
    }

    void DrawHelpWindow()
    {
        EditorGUILayout.HelpBox("Help!", MessageType.Info);

        GUILayout.BeginVertical();

        GUILayout.Label("Nothing is implemented yet, what do you want help with?");

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Back"))
        {
            CurrentSubwindow = CGODSubwindow.MainWindow;
        }

        GUILayout.EndVertical();
    }


    enum CGODSubwindow
    {
        MainWindow,
        Config,
        Help,
        MassUpdateConfirmation,
        Items,
        Monsters,
        Races,
        Classes,
        Abilities,
        Buffs,
    }

}
