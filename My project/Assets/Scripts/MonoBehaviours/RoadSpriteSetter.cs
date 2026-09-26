using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class RoadSpriteSetter : MonoBehaviour {
    [SerializeField] Sprite roadStraight2, roadCorner2, road3, road4;
    SpriteRenderer spriteRenderer;
    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void ChangeSprite(Direction d) {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        int count = Convert.ToString((int)d, 2).Count(c => c == '1');

        switch (count) {
            case 1:
            case 2:
                if (d.HasFlag(Direction.North) || (count == 1 && d.HasFlag(Direction.South))) {
                    if (d.HasFlag(Direction.South) || (count == 1 && d.HasFlag(Direction.North))) {
                        // N-S
                        spriteRenderer.sprite = roadStraight2;
                        transform.localRotation = Quaternion.Euler(0,0,0);
                        break;
                    }

                    if (d.HasFlag(Direction.East)) {
                        // N-E
                        spriteRenderer.sprite = roadCorner2;
                        transform.localRotation = Quaternion.Euler(0,0,0);
                        break;
                    }

                    // N-W
                    spriteRenderer.sprite = roadCorner2;
                    transform.localRotation = Quaternion.Euler(0,0,90);
                    break;
                }
                
                if (d.HasFlag(Direction.East) || (count == 1 && d.HasFlag(Direction.West))) {
                    if (d.HasFlag(Direction.West) || (count == 1 && d.HasFlag(Direction.East))) {
                        // E-W
                        spriteRenderer.sprite = roadStraight2;
                        transform.localRotation = Quaternion.Euler(0,0,-90);
                        break;
                    }
                    // E-S
                    spriteRenderer.sprite = roadCorner2;
                    transform.localRotation = Quaternion.Euler(0,0,-90);
                    break;
                }

                // S-W
                spriteRenderer.sprite = roadCorner2;
                transform.localRotation = Quaternion.Euler(new(0,0,180));
                break;
            case 3:
                spriteRenderer.sprite = road3;
                if (d.HasFlag(Direction.North)) {
                    if (!d.HasFlag(Direction.South)) {
                        // W-N-E
                        transform.localRotation = Quaternion.Euler(0,0,90);
                        break;
                    }
                    if (d.HasFlag(Direction.East)) {
                        // N-E-S
                        transform.localRotation = Quaternion.Euler(0,0,0);
                        break;
                    }
                    // N-W-S
                    transform.localRotation = Quaternion.Euler(0,0,180);
                    break;
                }
                // E-S-W
                transform.localRotation = Quaternion.Euler(0,0,-90);
                break;
            case 0:
            case 4:
                // N-E-S-W
                spriteRenderer.sprite = road4;
                transform.localRotation = Quaternion.Euler(0,0,0);
                break;
            default:
                Debug.Log("Invalid number of road directions set");
                break;
        }
    }
}