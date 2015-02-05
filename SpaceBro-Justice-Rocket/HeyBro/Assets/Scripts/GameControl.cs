using UnityEngine;
using System.Collections;

public class GameControl : MonoBehaviour {

    //basic setup
	public SequenceControls player;  //player script
	public EnemyControls enemy; 	// enemy script
	public GameObject playerLeft, playerRight; //player characters
    public AudioControl mainAudio; //audio for win/loss and move success

    //Game State
    public enum GamePhase {Intro, gameStart, startplayerTurn, playerTurn, startPlayerAttack, playerAttack, 
        startEnemyTurn, enemyCharge, enemyAttack, Counter, EndEnemyAttack, gameOver};
    public GamePhase gameTurn;

    //Input and sequence check
    public bool seqGenerated; 		// true if a sequence has been generated but not completed 
    public bool hasResetInput;		// Used to detect if we've reset input already

    //effects and animations
    public cameraShake mainCamera;
    public Animator cutsceneAnim, enemyAnimations;
    public Particle_Deactivate playerParticles;	

    //status of moves
    public bool pictogramsInRange, pictogramsFailed, canCounter;
    public Prompts prompts;

    //enemy attack stuff
    public int attackNum, counterNum;
    public Enemy_Particles enemyParticleParent;

    	
	void Start () {
        gameTurn = GamePhase.Intro;
	}

    public void GameStart()
    {
        gameTurn = GamePhase.gameStart;
    }

    void Update()
    {
        //control everything and ensure no turn overlaps
        switch (gameTurn)
        {
            case GamePhase.gameStart:
                hasResetInput = false;
		        startPlayerTurn();
                break;

            //player turn
            case GamePhase.startplayerTurn:
                startPlayerTurn();
                break;
            case GamePhase.playerTurn:
                PlayersTurn();
                break;
            case GamePhase.startPlayerAttack:
                StartPlayerAttack();
                break;
            
            //enemy turn
            case GamePhase.enemyCharge:
                EnemyTurn();
                break;
            case GamePhase.EndEnemyAttack:
                EnemyAttackOver();
                break;
        }
    }

    //check if game should end
    void CheckDeath()
    {
        if (gameTurn != GamePhase.gameOver)
        {
            //enemy lost
            if (enemy.hp <= 0)
            {
                gameTurn = GamePhase.gameOver;
                Invoke("loadSplashScreen", 5.0f);
                prompts.win();
                enemyAnimations.SetTrigger("ZeroHealth");
            }
            //players lost
            else if (player.hp <= 0)
            {
                gameTurn = GamePhase.gameOver;
                Invoke("loadSplashScreen", 5.0f);
                prompts.lose();
                enemyAnimations.SetTrigger("Laugh");
            }
        }
    }

    /* --------------------------------------------------------------------------------------------------------------------------
     * ~ PLAYER'S TURN
     * (1)	Generate a sequence
     * (2)	Check if player succeeds current move
     * (3)	check if finished seq
     * (4)	check enemy HP (if 0 => win game)
     * 
     * ~ ENEMY'S TURN 
     * (6)	randomly generate an attack	
     * (7)	if player counters within counter window => counter minigame
     * (8)	else if player blocks within block window => nothing happens
     * (9)	else if player fails => damage player
     * 
     * ~ GENERAL CHECKS
     * (10)	check enemy HP (if 0 => win game)
     * (11)	check player HP (if 0 => lose game)
     * PLAYER'S TURN AGAIN
     * -------------------------------------------------------------------------------------------------------------------------- */


    void startPlayerTurn()
    {
        CancelInvoke("startPlayerTurn");
        seqGenerated = false;
        gameTurn = GamePhase.playerTurn;
    }

    void PlayersTurn()
    {
        // (1) generate a sequence
        if (!seqGenerated)
        {
            player.generateSequence();
            seqGenerated = true;
        }
        else if (!hasResetInput) 
        {
			if (pictogramsInRange) {
				player.detectedA = -1;
				player.detectedB = -2;
				hasResetInput = true;
			}
		}

        // (2) check pass/fail for regular inputs
        if (pictogramsFailed)
        {
            pictogramsFailed = false;
        }
        else if (player.checkBothEvents() & pictogramsInRange)
        {
            hasResetInput = false;
            player.onSuccess();
            pictogramsInRange = false;
            player.correctMoves++;

            if (player.correctMoves < player.seqMoves)
            {
                mainAudio.playASound(0);
            }
            else if (player.correctMoves == player.seqMoves)
            {
                mainAudio.playASound(1);
                prompts.sequenceSuccess();
                gameTurn = GamePhase.startPlayerAttack;
            }
        }
    }

    public void StartPlayerAttack()
    {
        cutsceneAnim.SetTrigger("Cutscene 1");
        gameTurn = GamePhase.playerAttack;
    }

    public void PlayerAttack()
    {
        playerLeft.GetComponent<PlayerAnimations>().SetAnim(3);
        playerRight.GetComponent<PlayerAnimations>().SetAnim(3);
        playerParticles.partVisible();
        enemyAnimations.SetTrigger("Hurt");
        enemy.DamageEnemy(player.seqMoves * 20);
        CheckDeath();
        if (gameTurn != GamePhase.gameOver)
        {
            Invoke("startEnemyTurn", 1.5f);
            gameTurn = GamePhase.startEnemyTurn;
        }
    }

    void startEnemyTurn()
    {
        Invoke("createBlockSequence", 1.3f);
        prompts.showPrompt(2);
    }

    void EnemyTurn()
    {
        if (player.defending)
        {
            if (player.checkBothEvents() && pictogramsInRange)
            {
                playerLeft.GetComponent<PlayerAnimations>().characterAnims.SetBool("Blocking", false);
                playerRight.GetComponent<PlayerAnimations>().characterAnims.SetBool("Blocking", false);
                player.onSuccess();
                pictogramsInRange = false;
                if (canCounter)
                {
                    CounterSequence();
                }
                else
                {
                    player.blocked = true;
                    checkBlocked();
                    prompts.showPrompt(4);
                }
            }
            if (pictogramsFailed)
            {
                playerLeft.GetComponent<PlayerAnimations>().characterAnims.SetBool("Blocking", false);
                playerRight.GetComponent<PlayerAnimations>().characterAnims.SetBool("Blocking", false);
                pictogramsFailed = false;
                checkBlocked();

            }
        }
    }
    void EnemyAttackOver()
    {
        CheckDeath();
        if (gameTurn == GamePhase.EndEnemyAttack)
        {
            //start a new player turn
            Invoke ("startPlayerTurn", 2);
        }
    }

    void CounterSequence()
    {
        gameTurn = GamePhase.Counter;
        enemyParticleParent.attackAudio.Stop();
        prompts.showPrompt(3);
        canCounter = false;
        player.defending = false;
        counterNum = attackNum + 1;
        if (GameObject.Find("Counters"))
        {
            GameObject.Find("Counters").GetComponent<CounterControl>().Invoke("StartCounter", 0.3f);
        }
        else
        {
            GameObject.Find("Counters(Clone)").GetComponent<CounterControl>().Invoke("StartCounter", 0.3f);
        }
        enemyAnimations.SetTrigger("FailCharge");
        enemyParticleParent.chargeVisible = false;
        player.contactA = 0;
        player.contactB = 0;
        player.defending = false;
    }

    private void createBlockSequence()
    {
        player.generateBlockSequence();

        //////put variables to choose which attack 
        attackNum = Random.Range(0, 3);
        switch (attackNum)
        {
            case 0:
                enemyAnimations.SetTrigger("StartCharge");
                break;
            case 1:
                enemyAnimations.SetTrigger("Siphon");
                break;
            case 2:
                enemyAnimations.SetTrigger("PunchCharge");
                break;
        }
        gameTurn = GamePhase.enemyCharge;
    }

    private void checkBlocked()
    {
        player.defending = false;
        enemyAnimations.SetTrigger("Attack");
        gameTurn = GamePhase.enemyAttack;
    }

	//enemy attacks
	public void LaserDamage() {
		if (!player.blocked) {
			player.hp -= 20;
			mainCamera.Shake();
			playerLeft.GetComponent<PlayerAnimations>().SetAnim(5);
			playerRight.GetComponent<PlayerAnimations>().SetAnim(5);          
		}
        gameTurn = GamePhase.EndEnemyAttack;
	}

	public void SiphonAttack () {
		if (!player.blocked) {
			enemyParticleParent.startSiphonParticles();
			player.hp -= 20;
			enemy.hp += 40;
			mainCamera.Shake();
			playerLeft.GetComponent<PlayerAnimations>().SetAnim(5);
			playerRight.GetComponent<PlayerAnimations>().SetAnim(5);
		}
        gameTurn = GamePhase.EndEnemyAttack;
	}

	public void PunchAttack () {
		if (!player.blocked) {
			player.hp -= 30;
			mainCamera.Shake();
			playerLeft.GetComponent<PlayerAnimations>().SetAnim(5);
			playerRight.GetComponent<PlayerAnimations>().SetAnim(5);
		}
        gameTurn = GamePhase.EndEnemyAttack;
	}

	private void loadSplashScreen () {
		Application.LoadLevel (0);
	}

}
