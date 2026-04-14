using Sandbox;

public static class VehicleRegistry
{
	public static List<PrefabFile> PreRegistry = new();
	public static IEnumerable<PrefabFile> Vehicles { get; private set; }
	public static void RegisterVehicles()
	{
		RegisterGameVehicles();
		Vehicles = PreRegistry;
	}

	/// <summary>
	/// Retrieves all vehicles of category. If Inverse, then retrieve vehicles NOT in that category.
	/// </summary>
	/// <param name="CategoryName"></param>
	/// <param name="Inverse"></param>
	/// <returns></returns>
	public static List<PrefabFile> GetVehiclesOfCategory(string CategoryName, bool Inverse)
	{
		List<PrefabFile> Res = new();
		if ( !Inverse )
		{
			foreach ( var vehicle in Vehicles )
			{
				if ( vehicle.GetScene().GetComponent<Vehicle>().Category == CategoryName )
				{
					Res.Add( vehicle );
				}
			}
		}
		else {
			foreach ( var vehicle in Vehicles )
			{
				if ( vehicle.GetScene().GetComponent<Vehicle>().Category != CategoryName )
				{
					Res.Add( vehicle );
				}
			}

		}
			return Res;
	}

	/// <summary>
	/// Register all the vehicles in the base (vanilla) game.
	/// YOU HAVE TO ADD TO THIS IF YOU ADDED A NEW VEHICLE IN THE ASSET EXPLORER THAT'S NOT PART OF UGC.
	/// </summary>
	private static void RegisterGameVehicles()
	{
		PreRegistry.Add(PrefabFile.Load("prefabs/vehicles/industrial/truck00.prefab"));
	}
}
