using System.Collections;
using System.Collections.Generic;

public interface IDamageAble<T> where T : class
{
    float _maxHP {  get; set; }
    float _currentHP {  get; set; }
    int _deadCount { get; set; }
    void Damage(float dmg, IEnumerable<T> objects);
    void Die();
    void Respawn();
}
