using Il2CppAssets.Scripts.PeroTools.Commons;
using Il2CppAssets.Scripts.PeroTools.Managers;

namespace ChartManager.Managers;

internal static class SoundManager
{
    internal static void RestoreMusic() => UIManager.PnlStage.PlayMusic();

    internal static void PlayTroveBGM() => Singleton<AudioManager>.instance.PlayBGM("TroveBgm-InARomanticMood-Lukyanov");

    internal static void PlayAchievementsBGM() => Singleton<AudioManager>.instance.PlayBGM("AchievementBgm-SweetVermouth-MusMus");

    internal static void PlayClick() => Singleton<AudioManager>.instance.PlayButtonClickedSfx(ClickSfxType.Click);

    internal static void PlaySwitch() => Singleton<AudioManager>.instance.PlayButtonClickedSfx(ClickSfxType.Switch);

    internal static void PlayBack() => Singleton<AudioManager>.instance.PlayButtonClickedSfx(ClickSfxType.Back);

    internal static void PlayConfirm() => Singleton<AudioManager>.instance.PlayButtonClickedSfx(ClickSfxType.Confirm);

    internal static void PlayCrystal() => Singleton<AudioManager>.instance.PlayButtonClickedSfx(ClickSfxType.Crystal);
}
