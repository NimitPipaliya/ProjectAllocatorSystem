import { CapitalizePipe } from './capitalize.pipe';

describe('CapitalizePipe', () => {
  let pipe : CapitalizePipe;

  beforeEach(()=>{
    pipe = new CapitalizePipe;
  });

  it('create an instance', () => {
    const pipe = new CapitalizePipe();
    expect(pipe).toBeTruthy();
  });

  it('should capatalize the first letter of each word in a string',() => {
    const result = pipe.transform('hello angular');
    expect(result).toBe("Hello Angular");
  });

  it('should handle an empty string',() => {
    const result = pipe.transform('');
    expect(result).toBe('');
  });

  it('should handle null value',()=>{
    const result = pipe.transform(null);
    expect(result).toBe(null);
  });

  it('should handle undefined value',()=>{
    const result = pipe.transform(undefined);
    expect(result).toBe(undefined);
  });

  it('shhould handle string with multiple spaces',()=>{
    const result = pipe.transform('   hello angular   ');
    expect(result).toBe('   Hello Angular   ');
  });
});
