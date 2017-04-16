Mettre LATuteur.json dans "mes documents"

Structure JSON

{  
   "version":"1.0",
   "date":"19/04/2016",
   "tuteurs":[  
      {  
         "scene":[String], // une scene ou ce tuteur va s'afficher
         "id":[String], // un id unique pour chaque tuteur
         "etapes":[  // etapes 
            {  
               "type":[String], // un type peur etre [Texte,Image,Video]
               "titre":[String], // un titre pour l'etape
               "contenu":[String], // le contenu peut etre 
                            [
							 un texte : une chaine de charactere
							 un lien pour une video file:///C:/...../video.ogg ou http://www.site.com/video.ogg
							 un lien pour une image file:///C:/...../image.jpg ou http://www.site.com/image.jpg
                            ]
               "taille":{"longueur":[Int],"largeur":[Int]}, // la taille du Box 
               "ancrage":{  
                  "posx":[Float],
                  "posy":[Float], // position X et Y du BOX (depend du preset de l'ancrage)
                  "preset":[Int] -> [1..9],
                  "angle":[Int] -> [1..9]
				  /*
				  1 : Direction Down Left
				  2 : Direction Down ↓
				  3 : Direction Down Right
				  4 : Direction Left ←
				  5 : Direction Middle
				  6 : Direction Right →
				  7 : Direction Top Left
				  8 : Direction Top ↑
				  9 : Direction Top Right
				   
				   Pour preset c'est le point d'ancrage
				   
				   Pour angle : c'est la direction du fleche qui pointe vers un objet 
				          * pour 5 : ne pointe vers rien
				  */
               }
            },
			.... More Steps
         ]
      },
      .... More Tuteurs
    ]
}