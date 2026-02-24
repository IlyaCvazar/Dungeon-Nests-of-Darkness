using Godot;
using System;

public partial class WelcomeScript : RichTextLabel
{
	private Timer _timer;
	private bool _animationCompleted = false;
	private int _initialShadowOffset;
	private const float ANIMATION_DURATION = 1.0f; // Длительность одного цикла анимации в секундах
	private float _animationElapsed = 0f;
	private bool _isShadowAnimating = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Получаем начальное значение тени с проверкой
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
	}

	private void _on_mouse_entered()
	{
		// Перезапускаем анимацию при наведении курсора
		VisibleCharacters = 0;
		_animationCompleted = false;
		_animationElapsed = 0f;
		_isShadowAnimating = false;

		// Сбрасываем тень к начальному значению
		AddThemeConstantOverride("shadow_offset_y", _initialShadowOffset);

		if (_timer != null)
		{
			_timer.Start();
			GD.Print("Анимация запущена: сброс символов и тени");
		}
	}

	private void _on_timer_timeout()
	{
		if (VisibleCharacters < GetTotalCharacterCount())
		{
			VisibleCharacters += 1;

			// Выводим прогресс каждые 5 символов или при достижении конца
			if (VisibleCharacters % 5 == 0 || VisibleCharacters == GetTotalCharacterCount())
			{
				GD.Print($"Анимация: {VisibleCharacters}/{GetTotalCharacterCount()} символов показано");
			}
		}
		else
		{
			if (!_animationCompleted)
			{
				StartLoopingShadowAnimation();
				_animationCompleted = true;
				GD.Print("Анимация текста завершена, начинается зацикленная анимация тени");
			}

			if (_timer != null)
			{
				_timer.Stop();
			}
		}
	}

	private void StartLoopingShadowAnimation()
	{
		_isShadowAnimating = true;
		_animationElapsed = 0f;
		GetTree().CreateTimer(0.016f).Timeout += _on_shadow_animation_step;
	}

	private void _on_shadow_animation_step()
	{
		if (!_isShadowAnimating) return;

		_animationElapsed += 0.016f;

		// Плавное изменение тени от начального значения до +10 и обратно
		float progress = _animationElapsed / ANIMATION_DURATION;
		float sineProgress = (Mathf.Sin(progress * Mathf.Tau) + 1f) / 2f; // Синусоида от 0 до 1 и обратно
		int currentShadowOffset = (int)(_initialShadowOffset + 10 * sineProgress);

		AddThemeConstantOverride("shadow_offset_y", currentShadowOffset);
		GD.Print($"Анимация тени: {currentShadowOffset}px");

		// Продолжаем анимацию бесконечно
		GetTree().CreateTimer(0.016f).Timeout += _on_shadow_animation_step;
	}

	private void StopLoopingShadowAnimation()
	{
		_isShadowAnimating = false;
		// При остановке возвращаем начальное значение
		AddThemeConstantOverride("shadow_offset_y", _initialShadowOffset);
		GD.Print("Анимация тени остановлена, восстановлено начальное значение");
	}

	private void _on_mouse_exited()
	{
		// Останавливаем анимацию тени при уходе курсора
		StopLoopingShadowAnimation();
		GD.Print("Курсор ушёл, анимация тени остановлена");
	}
}
