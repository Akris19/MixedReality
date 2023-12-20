#define pot A0

int potValue = 0;
float mapped = 0;

void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
  pinMode(pot, INPUT);
}

void loop() {
  // put your main code here, to run repeatedly:
  potValue = analogRead(pot);
  //Serial.println(potValue);
  mapped = map(potValue, 0, 1024, 0, 100);
  mapped /=  100;
  Serial.println(mapped);
  delay(100);
}
