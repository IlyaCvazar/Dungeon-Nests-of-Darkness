using Godot;
using System.Threading.Tasks;

public partial class Mob : CharacterBody2D
{

	[Export]
	public float Speed = 100.0f;

	private Node2D _target;
	public bool _isAlive = true;
	private Sprite2D _sprite;

	public override void _Ready()
	{
		// Безопасный поиск дома
		_target = GetNode("../House") as Node2D;
		if (_target == null)
		{
			GD.PrintErr("Узел 'House' не найден!");
			return;
		}
		// Безопасный поиск спрайта
		_sprite = GetNodeOrNull<Sprite2D>("Sprite2D");
		if (_sprite == null)
		{
			GD.Print("Спрайт 'Sprite2D' не найден. Визуальный эффект смерти будет пропущен.");
		}

		Velocity = GlobalPosition.DirectionTo(_target.GlobalPosition) * Speed;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!_isAlive)
			return;

		MoveAndSlide();

		// Дополнительная проверка: дом найден и жив
		if (_target != null && _target.IsInsideTree())
		{
			float distanceToHouse = GlobalPosition.DistanceTo(_target.GlobalPosition);
			if (distanceToHouse <= 10.0f)
			{
				Die();
			}
		}
		else
		{
			// Дом удалён или не найден — останавливаем моба
			Velocity = Vector2.Zero;
			_isAlive = false;
			GD.PrintErr("Дом больше не доступен. Моб останавливается.");
		}
	}

	public async void Die()
	{
		if (!_isAlive) return;
		_isAlive = false;
		Velocity = Vector2.Zero;

		// Применяем визуальный эффект только если спрайт найден
		if (_sprite != null)
		{
			_sprite.Modulate = Colors.Red;
		}

		GD.Print("Моб умер у дома!");

		await ToSignal(GetTree().CreateTimer(1.0), "timeout");
		QueueFree();
	}
}
