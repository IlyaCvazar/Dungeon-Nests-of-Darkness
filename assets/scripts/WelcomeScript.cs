using Godot;
using System;

public partial class WelcomeScript : RichTextLabel
{
	private Timer _timer;
	private Timer _restartTimer;
	private bool _animationCompleted = false;
	private int _initialShadowOffset;
	private const float ANIMATION_DURATION = 1.0f;
	private float _animationElapsed = 0f;
	private bool _isShadowAnimating = false;
	private bool _isReverseAnimation = false;
	private bool _isWaitingForReverse = false;

	// Таймер для анимации тени — один экземпляр
	private Timer _shadowAnimationTimer;

	public override void _Ready()
	{
		Theme currentTheme = GetTheme();
		if (currentTheme != null)
		{
			_initialShadowOffset = GetThemeConstant("shadow_offset_y", "Label");
			GD.Print($"Начальное значение shadow_offset_y: {_initialShadowOffset}");
		}
		else
		{
			_initialShadowOffset = 0;
			GD.Print("Тема не найдена, начальное значение shadow_offset_y = 0");
		}

		VisibleCharacters = 0;
		_timer = GetNode<Timer>("../Timer");

		// Таймер задержки перед обратной анимацией
		_restartTimer = new Timer();
		_restartTimer.WaitTime = 5.0f;
		_restartTimer.OneShot = true;
		_restartTimer.Timeout += _on_restart_timer_timeout;
		AddChild(_restartTimer);

		// Таймер анимации тени
		_shadowAnimationTimer = new Timer();
		_shadowAnimationTimer.WaitTime = 0.016f; // ~60 FPS
		_shadowAnimationTimer.Autostart = false;
		// Удаляем строку с ProcessCallback
		_shadowAnimationTimer.Timeout += _on_shadow_animation_step;
		AddChild(_shadowAnimationTimer);

		_timer.Start();
		GD.Print("Анимация запущена при загрузке сцены");
	}


	private void _on_timer_timeout()
	{
		if (!_isReverseAnimation && !_isWaitingForReverse)
		{
			// Прямая анимация
			if (VisibleCharacters < GetTotalCharacterCount())
			{
				VisibleCharacters += 1;
				if (VisibleCharacters % 5 == 0 || VisibleCharacters == GetTotalCharacterCount())
				{
					GD.Print($"Прямая анимация: {VisibleCharacters}/{GetTotalCharacterCount()} символов показано");
				}
			}
			else
			{
				// Завершение прямой анимации
				if (!_animationCompleted)
				{
					StartLoopingShadowAnimation();
			_animationCompleted = true;
			_isWaitingForReverse = true;
			_restartTimer.Start();
			GD.Print("Прямая анимация завершена, запускается таймер задержки (10 секунд) перед обратной анимацией");
			_timer.Stop();
				}
			}
		}
		else if (_isWaitingForReverse)
		{
			// Ожидание — ничего не делаем, ждём срабатывания _restartTimer
		}
		else
		{
			// Обратная анимация
			if (VisibleCharacters > 0)
			{
				VisibleCharacters -= 1;
				if (VisibleCharacters % 5 == 0 || VisibleCharacters == 0)
				{
					GD.Print($"Обратная анимация: осталось {VisibleCharacters} символов");
				}
			}
			else
			{
				// Завершение обратной анимации
				StopLoopingShadowAnimation();

				_animationCompleted = false;
				_isReverseAnimation = false;
				_isWaitingForReverse = false;
				_animationElapsed = 0f;

				AddThemeConstantOverride("shadow_offset_y", _initialShadowOffset);
				GD.Print("Обратная анимация завершена, начинаем заново");

				_timer.Start();
			}
		}
	}

	private void _on_restart_timer_timeout()
	{
		GD.Print("Таймер задержки сработал — начинаем обратную анимацию");

		_isWaitingForReverse = false;
		_isReverseAnimation = true;
		StopLoopingShadowAnimation(); // Останавливаем анимацию тени
		_timer.Start();
	}

	private void StartLoopingShadowAnimation()
	{
		_isShadowAnimating = true;
		_animationElapsed = 0f;
		_shadowAnimationTimer.Start(); // Запускаем один таймер
	}

	private void _on_shadow_animation_step()
	{
		if (!_isShadowAnimating) return;

		_animationElapsed += 0.016f;

		float progress = _animationElapsed / ANIMATION_DURATION;
		float sineProgress = (Mathf.Sin(progress * Mathf.Tau) + 1f) / 2f;
		int currentShadowOffset = (int)(_initialShadowOffset + 10 * sineProgress);

		AddThemeConstantOverride("shadow_offset_y", currentShadowOffset);
		// Убираем частые GD.Print для производительности
	}

	private void StopLoopingShadowAnimation()
	{
		_isShadowAnimating = false;
		_shadowAnimationTimer.Stop(); // Останавливаем таймер
		AddThemeConstantOverride("shadow_offset_y", _initialShadowOffset);
	}

	// Очистка ресурсов при уничтожении узла
	public override void _ExitTree()
	{
		// Отменяем подписки на события
		if (_restartTimer != null)
		{
			_restartTimer.Timeout -= _on_restart_timer_timeout;
		}
		if (_shadowAnimationTimer != null)
		{
			_shadowAnimationTimer.Timeout -= _on_shadow_animation_step;
		}

		// Удаляем таймеры из дерева сцены
		_restartTimer?.QueueFree();
		_shadowAnimationTimer?.QueueFree();
	}
}
