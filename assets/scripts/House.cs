using Godot;

public partial class House : Area2D
{
	private CollisionShape2D _collisionShape;

	public override void _Ready()
	{
		_collisionShape = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");

		if (_collisionShape == null)
		{
			GD.PrintErr("Коллизия дома не найдена! Проверьте иерархию узлов.");
			return;
		}

		Monitoring = true;
		Monitorable = true;

		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body is Mob mob && mob._isAlive && mob.IsInsideTree())
		{
			GD.Print($"Моб {mob.GetInstanceId()} вошёл в зону дома и умирает.");
			mob.Die();
		}
		else if (body is Mob)
		{
			GD.Print($"Объект вошёл в зону дома, но это уже мёртвый моб.");
		}
		else
		{
			GD.Print($"Неизвестный объект вошёл в зону дома: {body.Name}");
		}
	}
}
