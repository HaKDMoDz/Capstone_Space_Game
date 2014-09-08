using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Galaxy : MonoBehaviour 
{
    private const int BACKGROUND_LAYER = 8;
    public GameObject theCore;

    public GameObject superMassiveBlackHole;
    public GameObject[] stars;
    public GameObject[] rockPlanets;
    public GameObject[] rockMoons;
    public GameObject[] gasGiants;
    public GameObject[] ggMoons;

    public List<PhysicsObject> poStars;
    public List<PhysicsObject> poTPlanets;
	public List<PhysicsObject> poTMoons;
	public List<PhysicsObject> poGPlanets;
	public List<PhysicsObject> poGMoons;

    public GameObject PlanetImOrbitting;

    public string debugText = "";

    private const int minNumStars = 10;
    private const int maxNumStars = 15;
    private int minStarRange = 8000;
    private int maxStarRange = 10000;
    private int StarRangeIncr;

    private const int minNumRockPlanetsPerStar = 2;
    private const int maxNumRockPlanetsPerStar = 4;
    private int minRockPlanetRange = 100;
    private int maxRockPlanetRange = 150;
    private int RockPlanetRangeIncr;

    private const int minNumMoonsPerRockPlanet = 0;
    private const int maxNumMoonsPerRockPlanet = 2;
    private int minRockMoonRange = 20;
    private int maxRockMoonRange = 30;
    private int RockMoonRangeIncr;

    private const int minNumGasGiants = 0;
    private const int maxNumGasGiants = 6;
    private int minGasGiantRange = 300;
    private int maxGasGiantRange = 750;
    private int GasGiantRangeIncr;

    private const int minNumMoonsPerGG = 1;
    private const int maxNumMoonsPerGG = 8;
    private int minGGMoonRange = 10;
    private int maxGGMoonRange = 25;
    private int GGMoonRangeIncr;

    //for adding randomness
    System.Random random = new System.Random();
    int starCount = 0;
    int rockPlanetCount = 0;
    int rockMoonCount = 0;
    int gasGiantCount = 0;
    int gasGiantMoonCount = 0;
    float randX = 0;
    float randY = 0;

    private PhysicsObject objectsInSimulation;

	private string[] planetNames = {"ARRAY_ELEMENT_0", "Monope", "Ospiea","Scuetov","Eystomia","Snouavis","Esbrypso","Dreuohines","Eachpichi","Bloasoarilia",
									"Eycaystora","Zurus","Uwhilia","Groenia","Iyprorth","Stauacury","Utswora","Scoieyama","Astvosie","Criokaocarro",
									"Ayyuywheron","Foclite","Egloria","Praiwei","Oacrillon","Plueegawa","Oysmoth","Preyomia","Ioclzuna","Groysuozuno",
									"Iyziuswypso","Reruta","Ocreshan","Scuatune","Aeshao","Breuonerth","Ecshomia","Chuutania","Uogrquna","Shuoceter",
									"Ienayskerth","Petune","Ocladus","Strealiv","Eglypso","Spoagawa","Urgloria","Traeotera","Eathcarvis","Bloaciurilia",
									"Aypeygruna","Quthea","Oslorth","Cleatera","Oicliri","Bloaagawa","Oksciea","Stuoonerth","Iysnjurn","Preacaotera",
									"Iawuclapus","Qunov","Ocromia","Tronerth","Eiclarth","Cluoutera","Oycroth","Snoeclite","Oyslqarth","Chiayiecarro",
									"Aymauscuna","Fanov","Estrosie","Straehines","Eusmion","Thoyucarro","Esshomia","Trueater","Ouslmarvis","Strayhiythea",
									"Uijoclion","Hoter","Ubrarvis","Bliynov","Augrides","Sheenus","Ofchilia","Cloualea","Iethtides","Smoykeiter","Uyyuycrapus",
									"Festea","Ushyke","Shuytune","Ublinda","Smiaupra","Ajswinda","Sneoonus","Agrviuq","Braytuahines","Uexuewhiri","Jastea",
									"Achyke","Snoynerth","Iasnilles","Stiaestea","Afstars","Thuyanov","Iygrhurn","Slaqeistea","Oyruibreron", "Daliv",
									"Uslio","Greatani","Obrir","Stoeanert","Amsnert","Smiuomi","Aiwhgapu","Whaoxuiste","Eguyscapu","Jari","Achar",
									"Cloyam","Aoflart","Druyuphu","Uiswot","Chioale","Iystryert","Skuitainu","Eybaoplio","Bani","Astio","Bliote",
									"Ueglide","Bliuoti","Arstrio","Skaoacarr","Autrmoll","Fruewuethe","Uwiepresha","Lecarr","Aslor","Dreito","Oysliu",
									"Stuaenu","Anprarvi","Smeieyam","Eystrjar","Chiataote","Eupeibliu","Yotur","Echarvi","Thuyphu","Aisnert","Snouato",
									"Ulthadu","Trieaphu","Iosnbyk","Fraiiete","Eamiospun","Vatani","Ufladu","Stuytani","Aopra","Skuyuzun","Obclo","Glaeayam",
									"Uiprnille","Snoyduate","Oloasnio","Rami","Uwhesha","Snuoni","Iyskar","Bloiuyam","Erclun","Plaionert","Iusnwar","Sloitoeto",
									"Ueiaesport","Xuste","Ustori","Preitun","Uisla","Flioatun","Epglero","Cruyanu","Ofrkesha","Straziatani","Uetudrio","Vegant",
									"Aslesha","Spoenu","Iawhyps","Blueecarr","Ujswesha","Swuovi","Aprlert","Stuikeorili","Oaheybrert","Yeclit","Egrono","Glaytani",
									"Usnar","Snouate","Ehcrille","Smaeeli","Oychgad","Glaifoagant","Eowaslyke","Dubo","Utrarvi","Blaeto","Iaprert","Craioru","Urflert",
									"Sceyahir","Upljosi","Playkauru","Ayfasnor","Guto","Estyps","Troitani","Aeclur","Breyeno","Ubbladu","Gleuuvi","Oprzeo","Whiafoani",
									"Iypuichero","Wete","Owhille","Sleuli","Ieprie","Grueobo","Opstror","Speiephu","Uyslzie","Whoitaote","Eugeustar","Wuzun","Uflon",
									"Driyste","Uiwheo","Sneuugaw","Ehglort","Sheaubo","Aeglbich","Creunoerut","Eopuyslar","Suni","Uslur","Criuto","Uispesha","Thoyehir",
									"Agprili","Snieater","Oflqot","Floejeile","Iagiesmosi","Lonop","Atrort","Trieto","Oytrille","Sheyute","Exbrie","Sciaenu","Ostfili",
									"Glioyiohir","Iutiudragu","Loni","Awhono","Chuahir","Oystresha","Whiuenu","Etwhapu","Flauani","Uosktori","Snagiuto","Ayxiochon",
									"Matur","Ushide","Prayte","Aestryk","Truyote","Obswarvi","Whouahir","Aofllille","Stiokoyphu","Uiqiyflyk","Xonu","Ogrono","Pluete",
									"Iysparvi","Gliouli","Orclosi","Freiular","Uiprror","Strekayvi","Aoquopromi","Bogant","Uspoll","Freuhine","Aepryk","Slueucarr",
									"Eisnor","Chuoatani","Iacrlor","Plioyoular","Oelousco","Panide","Usloll","Sheypr","Augrarvi","Griuahir","Erblur","Whuoute",
									"Aechqoll","Stuafionert","Afoythot","Sano","Adradu","Glater","Iaflir","Priyunide","Obsnart","Stoaarili","Iosmmun","Gloyyaecarr",
									"Oefiaprio","Govi","Etha","Trueli","Iascosi","Sluepr","Oxgleo","Gloonu","Iugrveo","Playgaupr","Apeusnero","Juyam","Aglori",
									"Theohine","Oasteo","Choaayam","Eyslapu","Dreoophu","Ieglmar","Gloybeyrut","Uavuythor","Qeti","Ugrori","Swumi","Oawhot",
									"Troyoste","Acsmesha","Treiocarr","Eiswqyk","Skeycoate","Augaugryk","Fanide","Eshori","Chaonert","Uiclot","Straoote","Actrero",
									"Smaoatur","Ustsillo","Glauqiomi","Eiresmono","Nacarr","Oswesha","Speyno","Ufloll","Whoyate","Englie","Grieali","Uashyind",
									"Scoekiozun","Eiboclori","Qorut","Estriu","Draizun","Ochipp","Snouale","Agswon","Fraeoru","Eusmvide","Smianoihir","Oymaiclind",
									"Jeyam","Oflun","Dreli","Oyshur","Clioarut","Uftrio","Droeovi","Uagrzillo","Striosoelar","Oiyoiscero","Legaw","Ofrille","Fraymi",
									"Oeblor","Sliyupr","Ukplot","Preyonu","Aothwyri","Trienuicur","Ohaublapu","Zuli","Uclir","Whuotun","Uoblur","Drieuti","Ebsca",
									"Glaonu","Oeshmio","Claoqani","Uexoytrert","Tenu","Eclar","Sluhine","Uichori","Scieorili","Uvclind","Slueetani","Eislqur",
									"Choydaiyam","Iyyaedro","Date","Etrille","Sciorut","Uowhir","Pruaato","Ajprot","Whaouli","Ouplsillo","Scuiceyphu","Aikoysmie",
									"Wale","Aprun","Stoiyam","Ayplyk","Brieuto","Ovdrori","Grooti","Ewhtar","Stauraicur","Uazuestra","Yozun","Ubresha","Whuter",
									"Aoswagu","Cloeetun","Ehpror","Sheaobo","Uchjille","Cloagoeter","Ezeythio","Facur","Echoll","Priaclit","Uistrart","Blaiurut",
									"Amswie","Sluyeyam","Easwpoll","Whoeqetani","Ayiaprie","Lephu","Eswyk","Criymi","Aybryk","Shaiugant","Erspor","Froyeste",
									"Ewhbesha","Shuydeithe","Ougauskio","Zagaw","Aslio","Skiyvi","Ouscarvi","Sliooto","Ewscot","Struouyam","Aytrvio","Swoatohine",
									"Oanoydryri","Jopr","Uglyk","Briycur","Iecrille","Spaiarut","Oystrarvi","Fruieto","Aoshtili","Stuejuyhir","Iefiebride","Qeto",
									"Eglori","Strali","Easlagu","Triaoter","Epslar","Drieawe","Ueplvyri","Sceameiclit","Eateuchar","Sagant","Oscero","Pleuwe",
									"Owhio","Treyucarr","Oxtrar","Crouocur","Iothwar","Praoxoile","Iupefrur","Xopr","Egrarvi","Creale","Oasnyri","Fraahir","Uwshyk",
									"Druaubo","Oyfrwide","Preywezun","Oiuystun","Juhine","Efragu","Clothe","Ueblar","Smuiuyam","Amsnoll","Plaoacarr","Oyprwot",
									"Stuyleiwe","Umiystarvi","Qoni","Uthili","Spoyno","Aowhar","Chueonert","Ohwhind","Swoazun","Uflvyps","Freuqatani","Iyqocrir",
									"Dorili","Ustradu","Blaole","Uoprot","Sliaapr","Evplio","Stoiorut","Uachgich","Gruigeonu","Oaseyglono","Sagant","Epla",
									"Gruiste","Oprar","Driuavi","Umstron","Thauutur","Uopryori","Thukuivi","Aylaibroll","Muru","Asmot","Sliobo","Uesposi","Frieonop",
									"Uxsloll","Sceoovi","Eystrhich","Bloimabo","Oetiyscyps","Gonu","Uflir","Gloahine","Aufrarvi","Smuyeni","Oqsmarvi","Criaoru",
									"Uyblmide","Sneagayter","Oizuflipp","Vunu","Ebride","Cragant","Uoswomi","Claoeli","Orshar","Spuari","Aiscmor","Cluapaepr",
									"Uayoyswili","Hali","Awharvi","Sheumi","Eustio","Fliaenert","Abgla","Plaiali","Uiswhili","Sloumaeno","Uituebrar","Peto","Aspie",
									"Snoytani","Oigrero","Straouru","Otdror","Thiaeno","Aiblkero","Truoleti","Iaqouskomi","Cete","Eflide","Slainert","Ieflyk",
									"Shiuuste","Ocswich","Spuyeyam","Ouwhtort","Criycurili","Eitiugrono","Woclit","Astrir","Draumi","Eablon","Shuionide","Ohscio",
									"Thoaoto","Ouslrille","Groayano","Aqaothori","Gewe","Escipp","Blaunu","Euswert","Fleyate","Edblili","Druiunide","Ouchpori",
									"Traozayli","Aibusparvi","Huphu","Uspart","Pliyyam","Eplie","Creaclit","Eystyk","Smoenide","Ablxosi","Trewounu","Iohiysmiu",
									"Wuclit","Usmyps","Smionert","Uadrert","Proyegaw","Oqthort","Sheouli","Aiflcipp","Trayoli","Oiteachio","Lunu","Esmeo","Swoacur",
									"Oystagu","Skiaumi","Ansmon","Sheuacarr","Iacryar","Draujianide","Oawucleo","Qari","Eflyri","Bloinop","Osnagu","Smuyanide",
									"Abstor","Swaiocur","Oaprfyps","Swueluphu","Uahoslilles","Dutur","Espart","Fliuru","Oisceo","Braoepr","Agfrert","Driootani",
									"Uyprgor","Scexucur","Aonachon","Sutur","Asnero","Pluazun","Uystrarvi","Shoaonide","Aystar","Strueovi","Uygrfagu","Skauyeli",
									"Ayliuscyk","Popr","Aswort","Griethe","Edrori","Dreyacur","Aptrun","Gloeogant","Aecrtad","Gloesiecur","Iomouscert","Qunide",
									"Usciu","Sniyle","Iygrori","Bloiabo","Eywhir","Snuoate","Egldomi","Gluejiytur","Aukoicrart","Korut","Aglarvi","Snoegaw","Iatrono",
									"Fruoste","Evtrur","Whoeste","Iuplyyk","Froideyru","Oeceaflar","Xahir","Astron","Smeori","Eogriu","Fleyeti","Ucclyps","Skayuti",
									"Aifrzomi","Smoegeini","Ouweitromi","Goste","Eskarvi","Bruaclit","Ouscon","Blaeunu","Alclio","Steuenu","Aythyun","Sceumiugant",
									"Oececlomi","Wagant","Aplot","Chaeru","Aiblyps","Creioli","Urgryri","Whuiobo","Oifrtyps","Slaudiotani","Asaeflili","Felar","Adrille",
									"Shuarili","Ouplort","Cleooni","Akwhar","Bleaogaw","Eclqarvi","Freojuerili","Ezoispiu","Pawe","Udrun","Shaogaw","Uaprert","Frueehir",
									"Uxblor","Snuoaru","Oasndono","Flayjoygaw","Apieplero","Hawe","Aclir","Chayhine","Aisnert","Glauati","Eyswili","Whoeurut","Iatrtad",
									"Traiwuru","Iadaflapu","Layam","Atha","Strayte","Iostrori","Truyecur","Uystar","Sleooclit","Eoflcar","Proyweular","Uarouwhon","Hecarr",
									"Ushur","Smeiclit","Eshur","Blaiehine","Alskipp","Smoyunu","Oushfapu","Fleaxuayam","Ayjotrot","Yuri","Uskert","Chuari","Ouscesha",
									"Groienide","Uhsmun","Shoueyam","Eaplyadu","Bluebierut","Oizuaskero","Yugaw","Odripp","Bleiclit","Uoshur","Briooru","Awglero",
									"Prayetun","Uesmlar","Shaiyiucarr","Uegiospipp","Lahine","Ubryri","Croite","Eosladu","Glooto","Ugtrapu","Steiarut","Ieswtesha",
									"Swaofule","Oaqoiblero","Wanide","Ofradu","Smiovi","Oythio","Croeote","Oiclio","Preaogaw","Ebrlur","Craeioyri","Aupoygreo","Nuru","Oclon",
									"Gliatun","Eflon","Smaeater","Ayplomi","Draoatur","Oeclbagu","Plauxaiti","Uysaclili","Iahir","Eswar","Groucarr","Aospor","Sluaupr","Udwhie",
									"Gleyato","Eitrxagu","Spuzeute","Uacuowho","Hucarr","Osmadu","Praezun","Aifryk","Triuunert","Uzslipp","Cleoayam","Aiflcot","Praenioru",
									"Uevuachar","Hucarr","Osmadu","Praezun","Aifryk","Triuunert","Uzslipp","Cleoayam","Aiflcot","Praenioru","Uevuachar","Saclit","Ospipp",
									"Smepr","Ayplich","Choeegaw","Ewpripp","Thoierili","Aeshgapu","Floamuopr","Aerebreo","Pozun","Acrarvi","Broaste","Uiscar","Friaati",
									"Encrori","Grieerut","Ostliu","Gluyjiozun", "Oufocrili"};

    public PhysicsObject getMainObject()
    {
        return objectsInSimulation;
    }
    PhysicsObject smbh_phy = new PhysicsObject();
    PhysicsObject star_phy, rockPlanet_phy, rockMoon_phy, gasGiant_phy, gasGiantMoon_phy;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

		poStars = new List<PhysicsObject>();
		poTPlanets = new List<PhysicsObject>();
		poTMoons = new List<PhysicsObject>();
		poGPlanets = new List<PhysicsObject>();
		poGMoons = new List<PhysicsObject>();

        createGalaxy();

    } //end method

    public string GetRandomSystem()
    {
        string returnVar = "";

        int numStars = poStars.Count;

        int randStar = UnityEngine.Random.Range(0, numStars - 1);

        returnVar = poStars[randStar].name;

        return returnVar;
    }

    private void createGalaxy()
    {
        StarRangeIncr = maxStarRange * 2;
        RockPlanetRangeIncr = maxRockPlanetRange * 2;
        RockMoonRangeIncr = maxRockMoonRange * 2;
        GasGiantRangeIncr = maxGasGiantRange * 2;
        GGMoonRangeIncr = maxGGMoonRange * 2;

        objectsInSimulation = new PhysicsObject(Vector2.zero, 0, PhysicsObjectType.GALAXY, random.Next(0, 360));
        objectsInSimulation.addComponent(smbh_phy);
        GameObject blackHole = GameObject.Instantiate(superMassiveBlackHole, new Vector3(0, 0, 0), Quaternion.LookRotation(Vector3.left)) as GameObject;
        blackHole.layer = BACKGROUND_LAYER;
        smbh_phy.myGameObject = blackHole;
        DontDestroyOnLoad(blackHole);
        blackHole.transform.parent = theCore.transform;

        // add stars
        starCount = random.Next(maxNumStars - minNumStars + 1) + minNumStars;

        int tempMinStarRange = minStarRange;
        int tempMaxStarRange = maxStarRange;
        for (int i = 0; i < starCount; i++)
        {
			int orbittingObjectCount = 0;
            minStarRange += StarRangeIncr;
            maxStarRange += StarRangeIncr;

            randX = random.Next((maxStarRange * -1), maxStarRange);
            while (randX > (minStarRange * -1) && randX < minStarRange)
            {
                randX = random.Next((maxStarRange * -1), maxStarRange);
            }

            randY = random.Next((maxStarRange * -1), maxStarRange);
            while (randY > (minStarRange * -1) && randY < minStarRange)
            {
                randY = random.Next((maxStarRange * -1), maxStarRange);
			}
            star_phy = new PhysicsObject(new Vector2(blackHole.transform.position.x + randX, blackHole.transform.position.y + randY), 11, PhysicsObjectType.STAR, random.Next(0, 360));
            star_phy.calcRadius(smbh_phy);
            smbh_phy.addComponent(star_phy);
            objectsInSimulation.addComponent(star_phy);
			star_phy.name = getRandomName();
			poStars.Add(star_phy);
			star_phy.name = getRandomName();
            GameObject starSystem = Instantiate(stars[UnityEngine.Random.Range(0, stars.Length)], new Vector3(star_phy.Position.x, 0, star_phy.Position.y), Quaternion.LookRotation(Vector3.left)) as GameObject;
            starSystem.layer = BACKGROUND_LAYER;
            star_phy.myGameObject = starSystem;
            starSystem.transform.parent = theCore.transform;
            starSystem.GetComponent<rotate>().myObject = star_phy;
            star_phy.rotateScript = starSystem.GetComponent<rotate>();

            //add rock planets to the stars
			rockPlanetCount = UnityEngine.Random.Range (minNumRockPlanetsPerStar, maxNumRockPlanetsPerStar);

            int tempMinRPRange = minRockPlanetRange;
            int tempMaxRPRange = maxRockPlanetRange;

            for (int j = 0; j < rockPlanetCount; j++)
            {
				orbittingObjectCount++;
                minRockPlanetRange += RockPlanetRangeIncr;
                maxRockPlanetRange += RockPlanetRangeIncr;

                // calculate randX between two ranges on equadistant sides of the centre
                randX = random.Next((maxRockPlanetRange * -1), maxRockPlanetRange);
                while (randX > (minRockPlanetRange * -1) && randX < minRockPlanetRange)
                {
                    randX = random.Next((maxRockPlanetRange * -1), maxRockPlanetRange);
                }

                // same for randY
                randY = random.Next((maxRockPlanetRange * -1), maxRockPlanetRange);
                while (randY > (minRockPlanetRange * -1) && randY < minRockPlanetRange)
                {
                    randY = random.Next((maxRockPlanetRange * -1), maxRockPlanetRange);
                }

                // create the planet then add it to the parent (star in this case) then add it to the objects the sim tracks
                rockPlanet_phy = new PhysicsObject(new Vector2(randX + star_phy.Position.x, randY + star_phy.Position.y), 222, PhysicsObjectType.ROCK_PLANET, random.Next(0, 360));
                rockPlanet_phy.calcRadius(star_phy);
                star_phy.addComponent(rockPlanet_phy);
                objectsInSimulation.addComponent(rockPlanet_phy);
                //GameObject RockPlanet = Instantiate(rockPlanets[UnityEngine.Random.Range(0, rockPlanets.Length - 1)], new Vector3(rockPlanet_phy.Position.x, 0, rockPlanet_phy.Position.y), Quaternion.LookRotation(Vector3.left)) as GameObject;
				poTPlanets.Add (rockPlanet_phy);
				rockPlanet_phy.name = star_phy.name + " " + getRomanNumeral(orbittingObjectCount);
				GameObject RockPlanet = Instantiate(rockPlanets[UnityEngine.Random.Range(0, rockPlanets.Length  -1)], new Vector3(rockPlanet_phy.Position.x, 0, rockPlanet_phy.Position.y), Quaternion.LookRotation(Vector3.left)) as GameObject;
				RockPlanet.layer = BACKGROUND_LAYER;
                RockPlanet.tag = "Planet";
                rockPlanet_phy.myGameObject = RockPlanet;
                RockPlanet.transform.parent = starSystem.transform;
                rockPlanet_phy.rotateScript = RockPlanet.GetComponent<rotate>();
                RockPlanet.GetComponent<rotate>().myObject = rockPlanet_phy;

                //add moons to the rock planets
                rockMoonCount = random.Next(maxNumMoonsPerRockPlanet - minNumMoonsPerRockPlanet + 1) + minNumMoonsPerRockPlanet;

                int tempMinRMRange = minRockMoonRange;
                int tempMaxRMRange = maxRockMoonRange;
                for (int k = 0; k < rockMoonCount; k++)
                {
                    minRockMoonRange += RockMoonRangeIncr;
                    maxRockMoonRange += RockMoonRangeIncr;

                    randX = random.Next((maxRockMoonRange * -1), maxRockMoonRange);
                    while (randX > (minRockMoonRange * -1) && randX < minRockMoonRange)
                    {
                        randX = random.Next((maxRockMoonRange * -1), maxRockMoonRange);
                    }

                    randY = random.Next((maxRockMoonRange * -1), maxRockMoonRange);
                    while (randY > (minRockMoonRange * -1) && randY < minRockMoonRange)
                    {
                        randY = random.Next((maxRockMoonRange * -1), maxRockMoonRange);
                    }

                    rockMoon_phy = new PhysicsObject(new Vector2(randX + rockPlanet_phy.Position.x, randY + rockPlanet_phy.Position.y), 3333, PhysicsObjectType.ROCK_MOON, random.Next(0, 360));
                    rockMoon_phy.calcRadius(rockPlanet_phy);
                    rockPlanet_phy.addComponent(rockMoon_phy);
                    objectsInSimulation.addComponent(rockMoon_phy);
					poTMoons.Add (rockMoon_phy);
					rockMoon_phy.name = rockPlanet_phy.name + " - " + getRandomName();
					GameObject RockMoon = Instantiate(rockMoons[UnityEngine.Random.Range(0, rockMoons.Length - 1)], new Vector3(rockMoon_phy.Position.x, 0, rockMoon_phy.Position.y), Quaternion.LookRotation(Vector3.left)) as GameObject;
                    RockMoon.layer = BACKGROUND_LAYER;
                    rockMoon_phy.myGameObject = RockMoon;
                    RockMoon.transform.parent = RockPlanet.transform;
                    rockMoon_phy.rotateScript = RockMoon.GetComponent<rotate>();
                    RockMoon.GetComponent<rotate>().myObject = rockMoon_phy;
                }// end rock moon for loop
                minRockMoonRange = tempMinRMRange;
                maxRockMoonRange = tempMaxRMRange;
            }// end rock planet for loop
            minRockPlanetRange = tempMinRPRange;
            maxRockPlanetRange = tempMaxRPRange;
            //add gas planets to the stars
            gasGiantCount = random.Next(maxNumGasGiants - minNumGasGiants + 1) + minNumGasGiants;

            int tempMinGGPlanet = minGasGiantRange;
            int tempMaxGGPlanet = maxGasGiantRange;
            for (int l = 0; l < gasGiantCount; l++)
            {
				orbittingObjectCount++;
                minGasGiantRange += GasGiantRangeIncr;
                maxGasGiantRange += GasGiantRangeIncr;

                randX = random.Next((maxGasGiantRange * -1), maxGasGiantRange);
                while (randX > (minGasGiantRange * -1) && randX < minGasGiantRange)
                {
                    randX = random.Next((maxGasGiantRange * -1), maxGasGiantRange);
                }

                randY = random.Next((maxGasGiantRange * -1), maxGasGiantRange);
                while (randY > (minGasGiantRange * -1) && randY < minGasGiantRange)
                {
                    randY = random.Next((maxGasGiantRange * -1), maxGasGiantRange);
                }

                gasGiant_phy = new PhysicsObject(new Vector2(randX + star_phy.Position.x, randY + star_phy.Position.y), 44444, PhysicsObjectType.GAS_GIANT, random.Next(0, 360));
                gasGiant_phy.calcRadius(star_phy);
                star_phy.addComponent(gasGiant_phy);
                objectsInSimulation.addComponent(gasGiant_phy);
				poGPlanets.Add (gasGiant_phy);
				gasGiant_phy.name = star_phy.name + " " + getRomanNumeral(orbittingObjectCount);
				GameObject GasGiant = Instantiate(gasGiants[UnityEngine.Random.Range(0, gasGiants.Length - 1)], new Vector3(gasGiant_phy.Position.x, 0, gasGiant_phy.Position.y), Quaternion.LookRotation(Vector3.left)) as GameObject;
                GasGiant.layer = BACKGROUND_LAYER;
				GasGiant.tag = "Planet";
                gasGiant_phy.myGameObject = GasGiant;
                GasGiant.transform.parent = starSystem.transform;
                gasGiant_phy.rotateScript = GasGiant.GetComponent<rotate>();
                GasGiant.GetComponent<rotate>().myObject = gasGiant_phy;

                //add moons to the gas planets
                gasGiantMoonCount = random.Next(maxNumMoonsPerGG - minNumMoonsPerGG + 1) + minNumMoonsPerGG;

                int tempMinGGMoonRange = minGGMoonRange;
                int tempMaxGGMoonRange = maxGGMoonRange;
                for (int m = 0; m < gasGiantMoonCount; m++)
                {
                    minGGMoonRange += GGMoonRangeIncr;
                    maxGGMoonRange += GGMoonRangeIncr;

                    randX = random.Next((maxGGMoonRange * -1), minGGMoonRange);
                    while (randX > (minGGMoonRange * -1) && randX < minGGMoonRange)
                    {
                        randX = random.Next((maxGGMoonRange * -1), minGGMoonRange);
                    }

                    randY = random.Next((maxGGMoonRange * -1), maxGGMoonRange);
                    while (randY > (minGGMoonRange * -1) && randY < minGGMoonRange)
                    {
                        randY = random.Next((maxGGMoonRange * -1), maxGGMoonRange);
                    }

                    gasGiantMoon_phy = new PhysicsObject(new Vector2(randX + gasGiant_phy.Position.x, randY + gasGiant_phy.Position.y), 555555, PhysicsObjectType.G_G_MOON, random.Next(0, 360));
                    gasGiantMoon_phy.calcRadius(gasGiant_phy);
                    gasGiant_phy.addComponent(gasGiantMoon_phy);
                    objectsInSimulation.addComponent(gasGiantMoon_phy);
					poGMoons.Add (gasGiantMoon_phy);
					gasGiantMoon_phy.name = gasGiant_phy.name + " - " + getRandomName();
					GameObject GGMoon = Instantiate(ggMoons[UnityEngine.Random.Range(0, ggMoons.Length - 1)], new Vector3(gasGiantMoon_phy.Position.x, 0, gasGiantMoon_phy.Position.y), Quaternion.LookRotation(Vector3.left)) as GameObject;
                    GGMoon.layer = BACKGROUND_LAYER;
                    GGMoon.tag = "Planet";
                    gasGiantMoon_phy.myGameObject = GGMoon;
                    GGMoon.transform.parent = GasGiant.transform;
                    gasGiantMoon_phy.rotateScript = GGMoon.GetComponent<rotate>();
                    GGMoon.GetComponent<rotate>().myObject = gasGiantMoon_phy;
                }// end gas giant moon for loop
                minGGMoonRange = tempMinGGMoonRange;
                maxGGMoonRange = tempMaxGGMoonRange;
            }// end gas giant for loop
            minGasGiantRange = tempMinGGPlanet;
            maxGasGiantRange = tempMaxGGPlanet;
        } //end star for loop
        minStarRange = tempMinStarRange;
        maxStarRange = tempMaxStarRange;
	
    }

	private string getRandomName()
	{
		int randomNameIndex = 0;
		string chosenName;
		do
		{
			randomNameIndex = UnityEngine.Random.Range (1, planetNames.Length - 1);
			chosenName = planetNames[randomNameIndex];

		}while (planetNames[randomNameIndex] == null);

		planetNames[randomNameIndex] = null;

		return chosenName;
	}

	private string getRomanNumeral(int number)
	{
		string romanNumeral = "";

		switch (number) 
		{
			case 1:
				romanNumeral = "I";
				break;
			case 2:
				romanNumeral = "II";
				break;
			case 3:
				romanNumeral = "III";
				break;
			case 4:
				romanNumeral = "IV";
				break;
			case 5:
				romanNumeral = "V";
				break;
			case 6:
				romanNumeral = "VI";
				break;
			case 7:
				romanNumeral = "VII";
				break;
			case 8:
				romanNumeral = "VIII";
				break;
			case 9:
				romanNumeral = "IX";
				break;
			case 10:
				romanNumeral = "X";
				break;
			case 11:
				romanNumeral = "XI";
				break;
			case 12:
				romanNumeral = "XII";
				break;
			case 13:
				romanNumeral = "XIII";
				break;
			case 14:
				romanNumeral = "XIV";
				break;
			case 15:
				romanNumeral = "XV";
				break;
			case 16:
				romanNumeral = "XVI";
				break;
			case 17:
				romanNumeral = "XXVII";
				break;
			case 18:
				romanNumeral = "XVIII";
				break;
			case 19:
				romanNumeral = "IXX";
				break;
			case 20:
				romanNumeral = "XX";
				break;

			default:
				romanNumeral = "WTF... Something isnt right with the number you gave to getRomanNumeral";
				break;
		}

		return romanNumeral;
	}
} //end class
