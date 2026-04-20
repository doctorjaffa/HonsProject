/* Functions for each UI element in the Menu scene */

using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void EnterSimulation()
    {
        SceneManager.LoadScene("Simulation");
    }

    // Dropdown menu changes which simulation file should be read into the NPC spawner
    public void ChangeSimulationFile(int index)
    {
        switch (index)
        {
            case 0:
                SimulationSettings.file = Resources.Load<TextAsset>("SimulationData_01");
                break;
            case 1:
                SimulationSettings.file = Resources.Load<TextAsset>("SimulationData_02");
                break;
            case 2:
                SimulationSettings.file = Resources.Load<TextAsset>("SimulationData_03");
                break;
            case 3:
                SimulationSettings.file = Resources.Load<TextAsset>("SimulationData_04");
                break;
            case 4:
                SimulationSettings.file = Resources.Load<TextAsset>("SimulationData_05");
                break;
            case 5:
                SimulationSettings.file = Resources.Load<TextAsset>("SimulationData_06");
                break;
            case 6:
                SimulationSettings.file = Resources.Load<TextAsset>("SimulationData_07");
                break;
        }
    }

    public void ChangeSimulationType(bool value)
    {
        SimulationSettings.isFuzzy = value;
    }

    public void ChangeSaveOption(bool value)
    {
        SimulationSettings.saveSimulation = value;
    }

    public void Exit()
    {
        Application.Quit();
    }
}
