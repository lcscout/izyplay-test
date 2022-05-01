using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour {
	public static event Action OnTapTouchableArea;
	public static event Action OnFirstTap;
	public static event Action<bool> OnPause;

	[Header("Initial Menu")]
	[SerializeField] private GameObject _initialMenu;
	[SerializeField] private TMP_Text _levelText;
	[SerializeField] private TMP_Text _coinsText;

	[Header("Levels Menu")]
	[SerializeField] private GameObject _levelsMenu;

	[Header("Skins Menu")]
	[SerializeField] private GameObject _skinsMenu;

	[Header("Pause/Restart Menu")]
	[SerializeField] private GameObject _pauseRestartMenu;
	[SerializeField] private GameObject _pauseButton;
	[SerializeField] private GameObject _continueButton;

	[Header("Game Over Menu")]
	[SerializeField] private GameObject _gameOverMenu;
	[SerializeField] private TMP_Text _gameOverCoinsText;

	private bool _areAnyMenusOpen = false;

	private void Awake() => _levelText.text = SceneManager.GetActiveScene().name;

	private void Start() => UpdateCoins(GameManager.Instance.Coins);

	private void OnEnable() {
		EndGame.OnEndGame += (mult) => IfScriptLoadedDo(() => _gameOverMenu.SetActive(true));
		PlayerController.OnDeath += () => IfScriptLoadedDo(() => _pauseRestartMenu.SetActive(true));
		GameManager.OnUpdateCoins += (coins) => UpdateCoins(coins);
		GameManager.OnSkinChange += CloseAnyMenu;
	}

	private void OnDisable() {
		EndGame.OnEndGame -= (mult) => IfScriptLoadedDo(() => _gameOverMenu.SetActive(true));
		PlayerController.OnDeath -= () => IfScriptLoadedDo(() => _pauseRestartMenu.SetActive(true));
		GameManager.OnUpdateCoins -= (coins) => UpdateCoins(coins);
		GameManager.OnSkinChange -= CloseAnyMenu;
	}

	public void TapTouchableArea() {
		if (_areAnyMenusOpen) {
			CloseAnyMenu();
			return;
		}

		OnTapTouchableArea?.Invoke();

		if (!GameManager.Instance.HasGameStarted)
			ConfigureInitialUI();
	}

	public void OpenLevelsMenu() {
		CloseAnyMenu();
		_levelsMenu.SetActive(true);

		_areAnyMenusOpen = true;
	}

	public void OpenSkinsMenu() {
		CloseAnyMenu();
		_skinsMenu.SetActive(true);

		_areAnyMenusOpen = true;
	}

	public void CloseAnyMenu() {
		_levelsMenu.SetActive(false);
		_skinsMenu.SetActive(false);

		_areAnyMenusOpen = false;
	}

	public void Continue() {
		if (SceneManager.GetActiveScene().buildIndex + 1 >= SceneManager.sceneCountInBuildSettings)
			GameManager.Instance.LoadLevel(0);

		GameManager.Instance.LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void Restart() => GameManager.Instance.LoadLevel(SceneManager.GetActiveScene().buildIndex);

	public void Pause(bool pauseState) {
		_continueButton.SetActive(pauseState);
		_pauseRestartMenu.SetActive(pauseState);

		OnPause?.Invoke(pauseState);
	}

	private void ConfigureInitialUI() {
		_initialMenu.SetActive(false);
		_pauseButton.SetActive(true);

		OnFirstTap?.Invoke();
	}

	private void UpdateCoins(int coins) {
		_coinsText.text = "x" + coins;
		_gameOverCoinsText.text = "x" + coins;
	}

	private void IfScriptLoadedDo(Action action) {
		if (this != null)
			action();
	}
}
