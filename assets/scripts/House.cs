using Godot;

public partial class House : Area2D
{
	[Export] public PackedScene BulletScene;
	[Export] public float ShootInterval = 1.0f;
	[Export] public float ShootRange = 500.0f;

	private CollisionShape2D _collisionShape;
	private Timer _shootTimer;
	private Node2D _targetMob;

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

		_shootTimer = new Timer();
		AddChild(_shootTimer);
		_shootTimer.WaitTime = ShootInterval;
		_shootTimer.OneShot = false;
		_shootTimer.Timeout += OnShootTimerTimeout;
		_shootTimer.Start();
	}

	private void OnBodyEntered(Node2D body)
	{
	if (body is Mob mob && mob.IsAlive && mob.IsInsideTree())
	{
		GD.Print($"Моб {mob.GetInstanceId()} вошёл в зону дома и начинает атаку.");
		mob.Die(); // вызываем метод атаки
	}
	}

	private void OnShootTimerTimeout()
	{
		_targetMob = FindNearestMob();
		if (_targetMob == null)
			return;

		if (BulletScene == null)
		{
			GD.PrintErr("BulletScene не назначена!");
			return;
		}

		Marker2D shootPoint = GetNodeOrNull<Marker2D>("ShootPoint");
		if (shootPoint == null)
		{
			GD.PrintErr("ShootPoint не найден!");
			return;
		}

		Bullet bullet = BulletScene.Instantiate<Bullet>();
		GetTree().Root.AddChild(bullet);
		bullet.GlobalPosition = shootPoint.GlobalPosition;

		Vector2 direction = (_targetMob.GlobalPosition - bullet.GlobalPosition).Normalized();
		bullet.Initialize(direction);
	}

	private Node2D FindNearestMob()
	{
		var mobs = GetTree().GetNodesInGroup("mobs");
		if (mobs.Count == 0)
			return null;

		Node2D nearest = null;
		float minDist = float.MaxValue;
		Vector2 housePos = GlobalPosition;

		foreach (Node2D mob in mobs)
		{
			if (!mob.IsInsideTree()) continue;
			float dist = housePos.DistanceSquaredTo(mob.GlobalPosition);
			if (dist < minDist && dist <= ShootRange * ShootRange)
			{
				minDist = dist;
				nearest = mob;
			}
		}

		return nearest;
	}
}
