
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
        EditorGUILayout.EndHorizontal();

        //BOTTOM ROW
        EditorGUILayout.BeginHorizontal();
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

        GUILayout.Label(
         "TODO - Add server host information here! ");//TODO

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

        GUILayout.Label(
            "Adding Content - To add content simply go to the valid location in the database folder and \n" +
            " > right click > data objects > content > any relevant item.");

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
