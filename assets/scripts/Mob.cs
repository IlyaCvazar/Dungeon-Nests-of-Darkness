using Godot;
using System.Threading.Tasks;

public partial class Mob : CharacterBody2D
{
	[Export] public float Speed { get; set; } = 100.0f;
	[Export] public int MaxHealth = 3;
	private int _currentHealth;

	private Node2D _target;
	private bool _isAlive = true;
	private AnimatedSprite2D _animatedSprite;
	private bool _reachedHouse = false; // Флаг: дошёл ли моб до дома
	private float _lastSpeed;

	public bool IsAlive => _isAlive;

	public override void _Ready()
	{
		_currentHealth = MaxHealth;
		_lastSpeed = Speed;

		_target = GetNode("../House") as Node2D;
		if (_target == null)
		{
			GD.PrintErr("Узел 'House' не найден!");
			return;
		}

		_animatedSprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
		if (_animatedSprite == null)
		{
			GD.Print("AnimatedSprite2D не найден.");
		}

		AddToGroup("mobs");
		Velocity = GlobalPosition.DirectionTo(_target.GlobalPosition) * Speed;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!_isAlive || _reachedHouse)
			return;
		if (Speed != _lastSpeed)
		{
			GD.Print($"Speed changed from {_lastSpeed} to {Speed}");
			_lastSpeed = Speed;
			// Если нужно, можно пересчитать направление или просто обновить Velocity, но это и так делается каждый кадр
		}

		if (_target == null || !_target.IsInsideTree())
		{
			Velocity = Vector2.Zero;
			_isAlive = false;
			GD.PrintErr("Дом исчез, моб останавливается.");
			return;
		}

		float distanceToHouse = GlobalPosition.DistanceTo(_target.GlobalPosition);
		if (distanceToHouse <= 10.0f)
		{
			Die(); // начинаем атаку дома
			return;
		}

		// Обновляем направление и скорость с учётом текущего Speed
		Vector2 direction = GlobalPosition.DirectionTo(_target.GlobalPosition);
		Velocity = direction * Speed;
		GD.Print($"Moving: Speed={Speed}, _reachedHouse={_reachedHouse}, distance={distanceToHouse}");

		MoveAndSlide();
	}

	public void TakeDamage(int damage)
	{
		if (!_isAlive) return;
		_currentHealth -= damage;
		GD.Print($"Моб получил урон, осталось здоровья: {_currentHealth}");

		if (_currentHealth <= 0)
		{
			DieForReal();
		}
	}

	private void DieForReal()
	{
		if (!_isAlive) return;
		_isAlive = false;
		Velocity = Vector2.Zero;

		var collision = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
		if (collision != null) collision.Disabled = true;

		if (_animatedSprite != null && _animatedSprite.SpriteFrames != null && _animatedSprite.SpriteFrames.HasAnimation("death"))
		{
			_animatedSprite.Play("death");
			_animatedSprite.AnimationFinished += () => QueueFree();
		}
		else
		{
			if (_animatedSprite != null)
				_animatedSprite.Modulate = Colors.Red;
			var timer = GetTree().CreateTimer(0.5f);
			timer.Timeout += QueueFree;
		}
		GD.Print("Моб умер!");
	}

public async void Die() // вызывается при касании дома
{
	if (!_isAlive || _reachedHouse) return;
	_reachedHouse = true;          // достиг дома – больше не двигаемся
	// _isAlive оставляем true, чтобы можно было получить урон
	Velocity = Vector2.Zero;

	if (_animatedSprite != null && _animatedSprite.SpriteFrames != null && _animatedSprite.SpriteFrames.HasAnimation("attack"))
	{
		_animatedSprite.Play("attack");
	}
	else
	{
		if (_animatedSprite != null)
			_animatedSprite.Modulate = Colors.Red;
		GD.Print("Анимация атаки не найдена.");
	}
	GD.Print("Моб достиг дома и атакует!");
	}
}
