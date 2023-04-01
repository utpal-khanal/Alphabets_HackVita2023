using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DrawingControllerAtoZ : MonoBehaviour
{
	public static int currentQuestion = 0; //////////////////////////////////////////////////////////
	Game game;///////////////////////////////////////////
	GameObject PhysicalDigit;////////////////////////////////////////////////////////////
	[SerializeField] GameObject platform;/////////////////////////////////////////////
	[SerializeField] GameObject[] positionToInstantiate; ////////////////////////////////////////
	[SerializeField] Vector3 centerPosition;/////////////////////////////////
	[SerializeField] int gameScore = 10;
	[SerializeField] TMPro.TMP_Text gameScoreHolder;
	[SerializeField] float DeadEndForDigits;
	[SerializeField] float deltaIncreaseAcceleration = 0.05f;
    [SerializeField] GameObject layerForTransition;
	[SerializeField] List<string> LettersToSpawnList;
    [SerializeField] float modifiedAcceleration = 8;
    [SerializeField] GameObject gameoverMenu;
    [SerializeField] int noOfLearning = 3;


	bool gamingMode = false;/////////////////////////////////////////////////
	int countForGamingMode = 0;/////////////////////////////////////
	bool playingTheGame = false;
	bool forNextScene = false;
	
	
	
	public int drawResolution = 1024;
	public int outputResolution = 28;

	public BoxCollider2D canvasCollider;
	public ComputeShader drawCompute;
	public float brushRadius;
	[Range(0, 1)]
	public float smoothing;
	RenderTexture canvas;
	RenderTexture outputCanvas;
	RenderTexture croppedCanvas;

	Camera cam;
	Vector2Int brushCentreOld;
	ComputeBuffer boundsBuffer;

    public void GameOverMenuLauch()
    {

        Instantiate(gameoverMenu);

    }

    void Start()
	{
		
		cam = Camera.main;
		ComputeHelper.CreateRenderTexture(ref canvas, drawResolution, drawResolution, FilterMode.Bilinear, ComputeHelper.defaultGraphicsFormat, "Draw Canvas");
		canvasCollider.gameObject.GetComponent<MeshRenderer>().material.mainTexture = canvas;

		ComputeHelper.CreateStructuredBuffer<uint>(ref boundsBuffer, 4);
		boundsBuffer.SetData(new int[] { drawResolution - 1, 0, drawResolution - 1, 0 });
		drawCompute.SetBuffer(0, "bounds", boundsBuffer);
		drawCompute.SetTexture(0, "Canvas", canvas);

		RandomQuestion(); //////////////////////////////////////////////////////////////////////////

		Physics.gravity = new Vector3(0f, -9.8f, 0);
		forNextScene = false;

	}

	public RenderTexture Canvas => canvas;

	public RenderTexture RenderOutputTexture()
	{

		ComputeHelper.CreateRenderTexture(ref outputCanvas, outputResolution, outputResolution, FilterMode.Point, ComputeHelper.defaultGraphicsFormat, "Draw Output");
		RenderTexture source = canvas;
		RenderTexture downscaleSource = RenderTexture.GetTemporary(source.width, source.height);
		Graphics.Blit(source, downscaleSource);
		int currWidth = source.width / 2;
		while (currWidth > outputResolution * 2)
		{
			RenderTexture temp = RenderTexture.GetTemporary(currWidth, currWidth);
			Graphics.Blit(downscaleSource, temp);
			currWidth /= 2;
			RenderTexture.ReleaseTemporary(downscaleSource);
			downscaleSource = temp;
		}
		Graphics.Blit(downscaleSource, outputCanvas);
		RenderTexture.ReleaseTemporary(downscaleSource);
		return outputCanvas;
	}

	bool giveQuestion = true;

	void Update()
	{
		if (forNextScene == true) return;

		if(playingTheGame)
        {
			if(giveQuestion)
            {
				RandomQuestion();
				giveQuestion = false;
            }

			if (Input.GetKeyDown(KeyCode.C))
			{
				//Debug.Log(GiveResult(NetworkConfidenceDisplay.predictedLabel));////////////////////////////////////////////////////////
				GiveResult(NetworkConfidenceDisplayAtoZ.predictedLabel);
				Clear();

			}

			if(PhysicalDigit != null && PhysicalDigit.transform.position.y <= DeadEndForDigits && gameScore > 0)
            {
				Destroy(PhysicalDigit);
			    gameScore -= 5;
				gameScoreHolder.text = gameScore.ToString();
				if(gameScore <= 0)
                {
					GoToNextScene();
				}
				else
                {
					RandomQuestion();
				}
				
            }

		}

		if (!gamingMode && Input.GetKeyDown(KeyCode.C))
		{
			//Debug.Log(GiveResult(NetworkConfidenceDisplay.predictedLabel));////////////////////////////////////////////////////////
			GiveResult(NetworkConfidenceDisplayAtoZ.predictedLabel);
			Clear();
			
		}

		Vector2 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);

		Bounds canvasBounds = canvasCollider.bounds;

		float tx = Mathf.InverseLerp(canvasBounds.min.x, canvasBounds.max.x, mouseWorld.x);
		float ty = Mathf.InverseLerp(canvasBounds.min.y, canvasBounds.max.y, mouseWorld.y);

		Vector2Int brushCentre = new Vector2Int((int)(tx * drawResolution), (int)(ty * drawResolution));

		drawCompute.SetInts("brushCentre", brushCentre.x, brushCentre.y);
		drawCompute.SetInts("brushCentreOld", brushCentreOld.x, brushCentreOld.y);
		drawCompute.SetFloat("brushRadius", brushRadius);
		drawCompute.SetFloat("smoothing", smoothing);
		drawCompute.SetInt("resolution", drawResolution);
		drawCompute.SetInt("mode", (Input.GetMouseButton(0)) ? 0 : 1);

		if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
		{
			ComputeHelper.Dispatch(drawCompute, drawResolution, drawResolution);
		}

		brushCentreOld = brushCentre;
	}

	void Clear()
	{
		boundsBuffer.SetData(new int[] { drawResolution - 1, 0, drawResolution - 1, 0 });
		ComputeHelper.ClearRenderTexture(canvas);
		
	}

	void OnDestroy()
	{
		ComputeHelper.Release(boundsBuffer);
		ComputeHelper.Release(canvas, outputCanvas, croppedCanvas);
	}

	void RandomQuestion() ////////////////////////////////////////////////////////////////
    {
		if(!gamingMode)
        {
			int a = Random.Range(0,LettersToSpawnList.Count);
			currentQuestion = LettersToSpawnList[a][0] -65; // A
			Debug.Log(currentQuestion);
			game = FindObjectOfType<Game>();
			PhysicalDigit = game.InstantiateADigit(currentQuestion);
		}
		else if(gamingMode)
        {
            int b = Random.Range(0, LettersToSpawnList.Count);
            currentQuestion = LettersToSpawnList[b][0] -65;
            Debug.Log(currentQuestion);
			game = FindObjectOfType<Game>();
			int a = Random.Range(0, 2);
			PhysicalDigit = game.InstantiateADigitForTheGame(currentQuestion, positionToInstantiate[a].transform.position);
			if(a==1)
            {
				PhysicalDigit.GetComponent<Rigidbody>().AddForce(new Vector3(centerPosition.x, centerPosition.y, centerPosition.z) * 20, ForceMode.Impulse);
			}
            else
            {
				PhysicalDigit.GetComponent<Rigidbody>().AddForce(new Vector3(-centerPosition.x, centerPosition.y, centerPosition.z) * 20, ForceMode.Impulse);
			}
		}
		
    }
    private void FixedUpdate()
    {
		if(gamingMode && playingTheGame && PhysicalDigit != null)
        {
			Rigidbody r = PhysicalDigit.GetComponent<Rigidbody>();

			

			if(r.velocity.y <= 0)
            {
				r.AddForce(new Vector3(0, modifiedAcceleration, 0), ForceMode.Acceleration);
			}
			
		}
	}

	void GiveResult(string a) ///////////////////////////////////////////////////////////////////
	{
		int yourAns = 0;

		if (a == "A") yourAns = 0;
		else if (a == "B") yourAns = 1;
		else if (a == "C") yourAns = 2;
		else if (a == "D") yourAns = 3;
		else if (a == "E") yourAns = 4;
		else if (a == "F") yourAns = 5;
		else if (a == "G") yourAns = 6;
		else if (a == "H") yourAns = 7;
		else if (a == "I") yourAns = 8;
		else if (a == "J") yourAns = 9;
		else if (a == "K") yourAns = 10;
		else if (a == "L") yourAns = 11;
		else if (a == "M") yourAns = 12;
		else if (a == "N") yourAns = 13;
		else if (a == "O") yourAns = 14;
		else if (a == "P") yourAns = 15;
		else if (a == "Q") yourAns = 16;
		else if (a == "R") yourAns = 17;
		else if (a == "S") yourAns = 18;
		else if (a == "T") yourAns = 19;
		else if (a == "U") yourAns = 20;
		else if (a == "V") yourAns = 21;
		else if (a == "W") yourAns = 22;
		else if (a == "X") yourAns = 23;
		else if (a == "Y") yourAns = 24;
		else if (a == "Z") yourAns = 25;

		if (yourAns == currentQuestion)
        {
			

			if (!gamingMode)
            {
				countForGamingMode++;
				//Debug.Log("Correct");
				if (PhysicalDigit.gameObject.GetComponent<MeshDestroy>() != null)
				{
					PhysicalDigit.gameObject.GetComponent<MeshDestroy>().destroyItOnce = true;
				}

				if(countForGamingMode != noOfLearning)
                {
					RandomQuestion();            ///////////////////////////////////////////////////////////////
				}
				else
                {
					gamingMode = true;
					Debug.Log("Lets Play game");
					Debug.Log("Making Transition");
					StartCoroutine(FirstTransitionTimeToPlayTheGame());
                }

			}

			else if(gamingMode && gameScore >= 0)
            {
				//Debug.Log("Correct");
				gameScore += 10;
				modifiedAcceleration = modifiedAcceleration - deltaIncreaseAcceleration;
				Debug.Log(gameScore);

				gameScoreHolder.text = gameScore.ToString();

				if (PhysicalDigit.gameObject.GetComponent<MeshDestroy>() != null)
				{
					PhysicalDigit.gameObject.GetComponent<MeshDestroy>().destroyItOnce = true;
				}

				RandomQuestion();            ///////////////////////////////////////////////////////////////
				
			}
			
		}
		else
        {
			if(!gamingMode)
            {
				//Debug.Log("No");
				//Debug.Log("currentQuestion" + currentQuestion + "and" + "yourAns" + yourAns);
			}
			else if(gamingMode)
            {
				//Debug.Log("Decrese the score");
				gameScore -= 5;
				
				Debug.Log(gameScore);
				gameScoreHolder.text = gameScore.ToString();
                if (gameScore <= 0)
                {
                    GameOverMenuLauch();
                }
            }
			
        }

	}

	IEnumerator FirstTransitionTimeToPlayTheGame()
    {
        CanvasGroup c = layerForTransition.GetComponent<CanvasGroup>();
        while (c.alpha < 1)
        {
            c.alpha = c.alpha + 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        Debug.Log("Foo");
        StageForGame();
        //StopCoroutine(FirstTransitionTimeToPlayTheGame());
    }
    IEnumerator SecondTransitionTimeToPlayTheGame()
    {
        CanvasGroup c = layerForTransition.GetComponent<CanvasGroup>();
        while (c.alpha > 0)
        {
            c.alpha = c.alpha - 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        //StopCoroutine(SecondTransitionTimeToPlayTheGame());

    }

    public void StageForGame()
    {
        Clear();
        platform.SetActive(false);
        playingTheGame = true;
        StartCoroutine(SecondTransitionTimeToPlayTheGame());
    }

    void GoToNextScene()
    {
        forNextScene = true;
        Debug.Log("next Scene");
    }


}
