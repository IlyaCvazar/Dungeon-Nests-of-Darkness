using Godot;
using System;

public partial class WelcomeScript : RichTextLabel
{
	private Timer _timer;
	private Timer _restartTimer;
	private bool _animationCompleted = false;
	private int _initialShadowOffset;
	private const float ANIMATION_DURATION = 1.0f; // Длительность одного цикла анимации в секундах
	private float _animationElapsed = 0f;
	private bool _isShadowAnimating = false;
	private bool _isReverseAnimation = false; // Флаг обратной анимации
	private bool _isWaitingForReverse = false; // Флаг ожидания перед обратной анимацией

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

		// Инициализируем таймер для задержки перед обратной анимацией (10 секунд)
		_restartTimer = new Timer();
		_restartTimer.WaitTime = 5.0f; // 10 секунд задержки
		_restartTimer.OneShot = true;
		_restartTimer.Timeout += _on_restart_timer_timeout;
		AddChild(_restartTimer);

		// Запускаем первую анимацию сразу после готовности
		_timer.Start();
		GD.Print("Анимация запущена при загрузке сцены");
	}

	private void _on_timer_timeout()
	{
		if (!_isReverseAnimation && !_isWaitingForReverse)
		{
			// Прямая анимация: показываем символы
			if (VisibleCharacters < GetTotalCharacterCount())
			{
				VisibleCharacters += 1;

				// Выводим прогресс каждые 5 символов или при достижении конца
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
					_isWaitingForReverse = true; // Переключаем в режим ожидания

					// Запускаем таймер задержки перед обратной анимацией
					_restartTimer.Start();
					GD.Print("Прямая анимация завершена, запускается таймер задержки (10 секунд) перед обратной анимацией");

					if (_timer != null)
			{
				_timer.Stop();
			}
		} // ЗАКРЫТИЕ if (!_animationCompleted)
	} // ЗАКРЫТИЕ else блока прямой анимации
}
else if (_isWaitingForReverse)
{
	// Ничего не делаем — ждём срабатывания _restartTimer
}
else
{
	// Обратная анимация: скрываем символы
	if (VisibleCharacters > 0)
	{
		VisibleCharacters -= 1;

		// Выводим прогресс каждые 5 символов или при достижении нуля
		if (VisibleCharacters % 5 == 0 || VisibleCharacters == 0)
		{
			GD.Print($"Обратная анимация: осталось {VisibleCharacters} символов");
		}
	}
	else
	{
		// Завершение обратной анимации — сбрасываем всё и начинаем заново
		StopLoopingShadowAnimation();

		_animationCompleted = false;
		_isReverseAnimation = false;
		_isWaitingForReverse = false;
		_animationElapsed = 0f;

		// Возвращаем начальное значение тени
		AddThemeConstantOverride("shadow_offset_y", _initialShadowOffset);

		GD.Print("Обратная анимация завершена, начинаем заново");

		// Запускаем анимацию заново
		_timer.Start();
	}
}
}

private void _on_restart_timer_timeout()
{
	GD.Print("Таймер задержки сработал — начинаем обратную анимацию");

	_isWaitingForReverse = false;
	_isReverseAnimation = true;

	// Останавливаем анимацию тени перед обратной анимацией
	StopLoopingShadowAnimation();

	// Запускаем таймер для обратной анимации
	_timer.Start();
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

	// Продолжаем анимацию бесконечно, пока не остановим
	GetTree().CreateTimer(0.016f).Timeout += _on_shadow_animation_step;
}

private void StopLoopingShadowAnimation()
{
	_isShadowAnimating = false;
	// При остановке возвращаем начальное значение
	AddThemeConstantOverride("shadow_offset_y", _initialShadowOffset);
	GD.Print("Анимация тени остановлена, восстановлено начальное значение");
}
}
