using UnityEngine;



public class GameManager : MonoBehaviour

{

  // Singleton instance

  private static GameManager _instance;

  

  public static GameManager Instance

  {

    get { return _instance; }

  }

  

  void Awake()

  {

    // Ensure only one instance exists

    if (_instance == null)

    {

      _instance = this;

      DontDestroyOnLoad(gameObject);

    }

    else

    {

      Destroy(gameObject);

    }

  } 

}