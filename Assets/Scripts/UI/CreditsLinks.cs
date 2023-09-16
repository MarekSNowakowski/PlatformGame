using UnityEngine;

public class CreditsLinks : MonoBehaviour
{
    [SerializeField]
    private MainMenuSoundsManager mainMenuSoundsManager;
    
    public void Developer()
    {
        mainMenuSoundsManager.PlayButtonClickedSound();
        Application.OpenURL("https://mareksnowakowski.github.io");
    }
    
    public void Environment()
    {
        mainMenuSoundsManager.PlayButtonClickedSound();
        Application.OpenURL("https://szadiart.itch.io/pixel-fantasy-caves");
    }
    
    public void Player()
    {
        mainMenuSoundsManager.PlayButtonClickedSound();
        Application.OpenURL("https://rvros.itch.io/animated-pixel-hero");
    }
    
    public void Music()
    {
        mainMenuSoundsManager.PlayButtonClickedSound();
        Application.OpenURL("https://tallbeard.itch.io/music-loop-bundle");
    }
    
    public void Icons()
    {
        mainMenuSoundsManager.PlayButtonClickedSound();
        Application.OpenURL("https://assetstore.unity.com/packages/2d/gui/icons/2d-simple-ui-pack-218050");
    }

    public void Sounds()
    {
        mainMenuSoundsManager.PlayButtonClickedSound();
        Application.OpenURL("https://assetstore.unity.com/packages/audio/sound-fx/rpg-essentials-sound-effects-free-227708");
    }
}
