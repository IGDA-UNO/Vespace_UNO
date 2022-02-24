using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemRelationButton : MonoBehaviour {

	public int OmekaItemID ; 
	public Text nameText;
	public Text relationText;
	public Image objectImage;
	public Button buttomComponent; 

    private float mainImageMaxWidth = 160 ; 
    private float mainImageMaxHeight = 160 ; 
	private PROUVE_OmekaPad omekaPad ; 
	
	public void SetupObject(OmekaItemRelationsObject itemRelationsObject, PROUVE_OmekaPad pad) {
		omekaPad = pad;
		objectImage.sprite = null ; 
		OmekaItemID = itemRelationsObject.subject_item_id;
		nameText.text = itemRelationsObject.subject_item_title ;
		relationText.text = itemRelationsObject.relation_text ; 
	}

	public void SetupSubject(OmekaItemRelationsSubject itemRelationsSubject, PROUVE_OmekaPad pad) {
		omekaPad = pad ; 
		objectImage.sprite = null ; 
		OmekaItemID = itemRelationsSubject.object_item_id;
		nameText.text = itemRelationsSubject.object_item_title ;
		relationText.text = itemRelationsSubject.relation_text ; 
	}

	public void displayThumbnail(PROUVE_ImageComponent imageComponent) {
		objectImage.sprite = imageComponent.returnSpriteToFit(mainImageMaxWidth,mainImageMaxHeight) ; 
	}

	public void displayThis() {
		omekaPad.displayItemInRelation(OmekaItemID) ; 
		//dataCaller.displayOmekaItemInRelation(OmekaItemID) ; 
	}
}
