using Godot;

public partial class Bullet : Area2D
{
	[Export] public float Speed = 400f;

	private Vector2 _direction;

	public void Initialize(Vector2 direction)
	{
		_direction = direction.Normalized();
	}

	public override void _PhysicsProcess(double delta)
	{
		Position += _direction * Speed * (float)delta;

		// Удаляем, если улетела слишком далеко
		if (GlobalPosition.Length() > 5000)
			QueueFree();
	}

	private void _on_body_entered(Node2D body)
	{
		if (body is Mob mob)
		{
			mob.TakeDamage(1); // наносим 1 урона
			QueueFree();       // пуля исчезает
		}
	}

	// Если вдруг нужна реакция на другие Area2D, можно оставить и area_entered,
	// но для мобов используем body_entered.
}
